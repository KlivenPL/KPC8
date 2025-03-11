using ExternalDevices._Infrastructure;
using ExternalDevices.HID;
using LightweightEmulator.Configuration;
using LightweightEmulator.ExternalDevices;
using System;
using System.Collections.Generic;
using Vortice.XInput;

namespace ExternalDevices.Lw {

    // Configuration now maps KPadButton to its analog address.
    public class LwKPadConfiguration : ILwKpcExternalDeviceConfiguration {
        public string PadName { get; init; }

        // Configurable digital mode addresses.
        public ushort DigitalPressAddress { get; init; }
        public ushort DigitalReleaseAddress { get; init; }

        // Configurable analog mapping: map from KPadButton to its analog address.
        public Dictionary<KPadButtons, ushort> AnalogMapping { get; init; } = new Dictionary<KPadButtons, ushort>();

        public void Configure(Action<ILwExternalDevice> addExternalDevice) {
            var lwKPad = new LwKPad(this);
            addExternalDevice(lwKPad);
        }
    }

    public class LwKPad : LwExternalDevice {
        private readonly RawInputManager rawInputManager;

        // Digital event sets for press (mode 0) and release (mode 1).
        private readonly HashSet<KPadButtons> digitalPressEvents;
        private readonly HashSet<KPadButtons> digitalReleaseEvents;
        // Track active keyboard keys.
        private readonly HashSet<VirtualKey> activeKeys;

        // For analog mode, record the time a KPadButton was first pressed.
        private readonly Dictionary<KPadButtons, DateTime> analogPressStart;

        // For XInput digital events we track previous state.
        private GamepadButtons previousXInputButtons;

        // --- Configurable addresses (set via configuration) ---
        private readonly ushort digitalPressAddress;
        private readonly ushort digitalReleaseAddress;
        // Now the analog mapping is a dictionary from KPadButtons to their analog address.
        private readonly Dictionary<KPadButtons, ushort> analogMapping;

        // Analog ramp time in milliseconds (time to reach full 255 value).
        private readonly double analogRampTimeMs = 500.0;

        public LwKPad(LwKPadConfiguration config) : base(config.PadName) {
            digitalPressAddress = config.DigitalPressAddress;
            digitalReleaseAddress = config.DigitalReleaseAddress;
            analogMapping = new Dictionary<KPadButtons, ushort>(config.AnalogMapping);

            // Add all configured addresses to the device's mapped addresses.
            MappedAddresses.Add(digitalPressAddress);
            MappedAddresses.Add(digitalReleaseAddress);
            foreach (var addr in analogMapping.Values)
                MappedAddresses.Add(addr);

            rawInputManager = RawInputManager.Instance;
            digitalPressEvents = new HashSet<KPadButtons>();
            digitalReleaseEvents = new HashSet<KPadButtons>();
            activeKeys = new HashSet<VirtualKey>();
            analogPressStart = new Dictionary<KPadButtons, DateTime>();
            previousXInputButtons = GamepadButtons.None;
        }

        /// <summary>
        /// Polls the device. The passed address determines which mode to operate in:
        /// - If the address equals DigitalPressAddress, returns one-shot press events.
        /// - If it equals DigitalReleaseAddress, returns one-shot release events.
        /// - Otherwise, it is treated as an analog key address.
        /// </summary>
        public override byte HandleLbext(ushort addr) {
            ProcessKeyboardEvents();
            ProcessXInputEvents();

            if (addr == digitalPressAddress) {
                return GetDigitalPressMode();
            } else if (addr == digitalReleaseAddress) {
                return GetDigitalReleaseMode();
            } else {
                return GetAnalogMode(addr);
            }
        }

        public override void HandleSbext(ushort address, byte data) {
            // Not used.
        }

        #region Event Processing

        private void ProcessKeyboardEvents() {
            while (rawInputManager.TryDequeue(out var rawEvent)) {
                if (rawEvent.IsRecognizedVirtualKey(out var vk)) {
                    var btn = ConvertVirtualKeyToKPadButton(vk);
                    if (rawEvent.IsKeyDown) {
                        if (!activeKeys.Contains(vk)) {
                            activeKeys.Add(vk);
                            digitalPressEvents.Add(btn);
                            // If this button is mapped in analog mode, record its press time.
                            if (analogMapping.ContainsKey(btn) && !analogPressStart.ContainsKey(btn)) {
                                analogPressStart[btn] = DateTime.Now;
                            }
                        }
                    } else { // Key up.
                        if (activeKeys.Contains(vk)) {
                            activeKeys.Remove(vk);
                            digitalReleaseEvents.Add(btn);
                        }
                        // Remove analog timing regardless.
                        if (analogPressStart.ContainsKey(btn)) {
                            analogPressStart.Remove(btn);
                        }
                    }
                }
            }
        }

        private void ProcessXInputEvents() {
            if (XInput.GetState(0, out var state)) {
                GamepadButtons currentButtons = state.Gamepad.Buttons;
                GamepadButtons newlyPressed = currentButtons & ~previousXInputButtons;
                GamepadButtons newlyReleased = previousXInputButtons & ~currentButtons;

                foreach (GamepadButtons button in Enum.GetValues(typeof(GamepadButtons))) {
                    var btn = ConvertGamepadButtonToKPadButton(button);
                    if (newlyPressed.HasFlag(button)) {
                        digitalPressEvents.Add(btn);
                        if (analogMapping.ContainsKey(btn) && !analogPressStart.ContainsKey(btn)) {
                            analogPressStart[btn] = DateTime.Now;
                        }
                    }
                    if (newlyReleased.HasFlag(button)) {
                        digitalReleaseEvents.Add(btn);
                        if (analogPressStart.ContainsKey(btn)) {
                            analogPressStart.Remove(btn);
                        }
                    }
                }
                previousXInputButtons = currentButtons;
            }
        }

        #endregion

        #region Mode Implementations

        // Mode 0: Digital press mode.
        private byte GetDigitalPressMode() {
            byte result = 0;
            foreach (var btn in digitalPressEvents) {
                result |= (byte)btn;
            }
            digitalPressEvents.Clear();
            return result;
        }

        // Mode 1: Digital release mode.
        private byte GetDigitalReleaseMode() {
            byte result = 0;
            foreach (var btn in digitalReleaseEvents) {
                result |= (byte)btn;
            }
            digitalReleaseEvents.Clear();
            return result;
        }

        // Mode 2: Analog mode.
        // For the given analog address, find the KPadButton mapped to it.
        // If that button is pressed, compute a ramp value (0–255) based on how long it has been held.
        private byte GetAnalogMode(ushort addr) {
            DateTime now = DateTime.Now;
            foreach (var pair in analogMapping) {
                // pair.Key is the KPadButton, pair.Value is its analog address.
                if (pair.Value == addr) {
                    var mappedButton = pair.Key;
                    if (analogPressStart.TryGetValue(mappedButton, out DateTime pressTime)) {
                        double heldMs = (now - pressTime).TotalMilliseconds;
                        return (byte)Math.Min(255, (heldMs / analogRampTimeMs) * 255);
                    }
                }
            }
            return 0;
        }

        #endregion

        #region Helper Converters

        private KPadButtons ConvertVirtualKeyToKPadButton(VirtualKey vk) {
            return vk switch {
                VirtualKey.A => KPadButtons.Left,
                VirtualKey.D => KPadButtons.Right,
                VirtualKey.S => KPadButtons.Down,
                VirtualKey.W => KPadButtons.Up,
                VirtualKey.Space => KPadButtons.A,
                VirtualKey.Shift => KPadButtons.B,
                VirtualKey.Tab => KPadButtons.Select,
                VirtualKey.Enter => KPadButtons.Start,
                _ => KPadButtons.None,
            };
        }

        private KPadButtons ConvertGamepadButtonToKPadButton(GamepadButtons btn) {
            if (btn.HasFlag(GamepadButtons.DPadUp)) return KPadButtons.Up;
            if (btn.HasFlag(GamepadButtons.DPadRight)) return KPadButtons.Right;
            if (btn.HasFlag(GamepadButtons.DPadDown)) return KPadButtons.Down;
            if (btn.HasFlag(GamepadButtons.DPadLeft)) return KPadButtons.Left;
            if (btn.HasFlag(GamepadButtons.A)) return KPadButtons.A;
            if (btn.HasFlag(GamepadButtons.B)) return KPadButtons.B;
            if (btn.HasFlag(GamepadButtons.Back)) return KPadButtons.Select;
            if (btn.HasFlag(GamepadButtons.Start)) return KPadButtons.Start;
            return KPadButtons.None;
        }

        public override void Dispose() {
            //rawInputManager.Dispose();
        }

        #endregion
    }
}

using ExternalDevices._Infrastructure;
using ExternalDevices.HID;
using LightweightEmulator.Configuration;
using LightweightEmulator.ExternalDevices;
using System;
using Vortice.XInput;

namespace ExternalDevices.Lw {
    public class LwKPadConfiguration : ILwKpcExternalDeviceConfiguration {
        public string PadName { get; init; }
        public ushort PadAddress { get; init; }

        public void Configure(Action<ILwExternalDevice> addExternalDevice) {
            var lwKPad = new LwKPad(PadAddress, PadName);
            addExternalDevice(lwKPad);
        }
    }

    public class LwKPad : LwExternalDevice {
        private readonly LowLevelKeyboardListener keyboardListener;
        private KPadButtons keyboardButtons = KPadButtons.None;
        private object keyboardButtonsLock = new();

        public LwKPad(ushort address, string name) : base(name) {
            MappedAddresses.Add(address);
            keyboardListener = new LowLevelKeyboardListener();
        }

        public override void Initialize() {
            keyboardListener.OnKeyPressed += OnKeyboardButtonPressed;
            keyboardListener.HookKeyboard();
        }

        public override byte HandleLbext(ushort _) {
            var buttons = keyboardButtons;

            if (XInput.GetState(0, out var state)) {
                var padButtons = state.Gamepad.Buttons;

                if (padButtons.HasFlag(GamepadButtons.DPadUp)) {
                    buttons |= KPadButtons.Up;
                }

                if (padButtons.HasFlag(GamepadButtons.DPadRight)) {
                    buttons |= KPadButtons.Right;
                }

                if (padButtons.HasFlag(GamepadButtons.DPadDown)) {
                    buttons |= KPadButtons.Down;
                }

                if (padButtons.HasFlag(GamepadButtons.DPadLeft)) {
                    buttons |= KPadButtons.Left;
                }

                if (padButtons.HasFlag(GamepadButtons.A)) {
                    buttons |= KPadButtons.A;
                }

                if (padButtons.HasFlag(GamepadButtons.B) || padButtons.HasFlag(GamepadButtons.LeftShoulder) || padButtons.HasFlag(GamepadButtons.RightShoulder)) {
                    buttons |= KPadButtons.B;
                }

                if (padButtons.HasFlag(GamepadButtons.Back)) {
                    buttons |= KPadButtons.Select;
                }

                if (padButtons.HasFlag(GamepadButtons.Start)) {
                    buttons |= KPadButtons.Start;
                }

            }

            lock (keyboardButtonsLock) {
                keyboardButtons = KPadButtons.None;
                return (byte)buttons;
            }
        }

        public override void HandleSbext(ushort address, byte data) { }

        private void OnKeyboardButtonPressed(object sender, KeyPressedArgs e) {
            lock (keyboardButtonsLock) {
                switch (e.KeyPressed) {
                    case VirtualKey.A:
                        keyboardButtons |= KPadButtons.Left;
                        break;
                    case VirtualKey.D:
                        keyboardButtons |= KPadButtons.Right;
                        break;
                    case VirtualKey.S:
                        keyboardButtons |= KPadButtons.Down;
                        break;
                    case VirtualKey.W:
                        keyboardButtons |= KPadButtons.Up;
                        break;
                    case VirtualKey.Space:
                        keyboardButtons |= KPadButtons.A;
                        break;
                    case VirtualKey.Shift:
                        keyboardButtons |= KPadButtons.B;
                        break;
                    case VirtualKey.Tab:
                        keyboardButtons |= KPadButtons.Select;
                        break;
                    case VirtualKey.Enter:
                        keyboardButtons |= KPadButtons.Start;
                        break;
                }
            }
        }
    }
}

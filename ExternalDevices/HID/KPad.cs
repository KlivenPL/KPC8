using Components._Infrastructure.IODevices;
using Components.Signals;
using ExternalDevices._Infrastructure;
using Infrastructure.BitArrays;
using _Infrastructure.Simulation.Updates;
using System;
using System.Collections;
using System.Text;
using Vortice.XInput;

namespace ExternalDevices.HID {
    public class KPad : IODeviceBase, IExternalDevice, IUpdate, IDisposable {
        private const int Size = 8;
        protected BitArray mainBuffer;
        private readonly LowLevelKeyboardListener keyboardListener;
        public BitArray Content => new(mainBuffer);
        public SignalPort ChipSelect { get; set; }
        public SignalPort ExtIn { get; set; }
        public SignalPort ExtOut { get; set; }
        public KPadButtons SimulatedButtons { get; set; } = KPadButtons.None;
        public int Priority => -2;

        private KPadButtons keyboardButtons = KPadButtons.None;

        public KPad(string name) : base(name) {
            mainBuffer = new(Size);
            keyboardListener = new LowLevelKeyboardListener();
            base.Initialize(0, Size);
        }

        void IExternalDevice.InitializeExternalDevice() {
            this.RegisterUpdate();
            keyboardListener.OnKeyPressed += OnKeyboardButtonPressed;
            keyboardListener.HookKeyboard();
        }

        private void OnKeyboardButtonPressed(object sender, KeyPressedArgs e) {
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

        public virtual void Update() {

            var buttons = SimulatedButtons | keyboardButtons;

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

            mainBuffer = BitArrayHelper.FromByteLE((byte)buttons);

            for (int i = 0; i < Size; i++) {
                Outputs[i].Write(mainBuffer[i]);
            }

            if (ChipSelect) {
                mainBuffer.SetAll(false);
                keyboardButtons = KPadButtons.None;
            }
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.AppendLine($"CS: {ChipSelect}, Buttons: {(KPadButtons)BitArrayHelper.ToByteLE(mainBuffer)}");

            return sb.ToString();
        }

        public void Dispose() {
            keyboardListener.UnHookKeyboard();
            this.UnregisterUpdate();
        }
    }
}

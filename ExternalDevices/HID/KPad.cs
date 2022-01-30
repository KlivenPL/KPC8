using Components._Infrastructure.IODevices;
using Components.Signals;
using Infrastructure.BitArrays;
using Simulation.Updates;
using System;
using System.Collections;
using System.Text;
using Vortice.XInput;

namespace ExternalDevices.HID {
    public class KPad : IODeviceBase, IExternalDevice, IUpdate {
        private const int Size = 8;
        protected BitArray mainBuffer;
        public BitArray Content => new(mainBuffer);
        public SignalPort ChipSelect { get; set; }
        public SignalPort ExtIn { get; set; }
        public SignalPort ExtOut { get; set; }
        public KPadButtons SimulatedButtons { get; set; } = KPadButtons.None;
        public int Priority => -2;

        public KPad(string name) : base(name) {
            mainBuffer = new(Size);
            base.Initialize(0, Size);
        }

        void IExternalDevice.InitializeExternalDevice() {
            ChipSelect.OnEdgeRise += ChipEnable_OnEdgeRise;
            this.RegisterUpdate();
        }

        public void SetContent(BitArray value) {
            for (int i = 0; i < mainBuffer.Length; i++) {
                mainBuffer[i] = value[i];
            }
        }

        private void ChipEnable_OnEdgeRise() {


            //mainBuffer.SetAll(false);
        }

        public virtual void Update() {
            var buttons = SimulatedButtons;

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

            if (Console.KeyAvailable) {
                var keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.E || keyInfo.Modifiers == ConsoleModifiers.Shift) {
                    buttons |= KPadButtons.B;
                }

                if (keyInfo.Key == ConsoleKey.Spacebar) {
                    buttons |= KPadButtons.A;
                }

                if (keyInfo.Key == ConsoleKey.Enter) {
                    buttons |= KPadButtons.Start;
                } else if (keyInfo.Key == ConsoleKey.Tab) {
                    buttons |= KPadButtons.Select;
                }

                if (keyInfo.Key == ConsoleKey.W) {
                    buttons |= KPadButtons.Up;
                }

                if (keyInfo.Key == ConsoleKey.A) {
                    buttons |= KPadButtons.Left;
                }

                if (keyInfo.Key == ConsoleKey.S) {
                    buttons |= KPadButtons.Down;
                }

                if (keyInfo.Key == ConsoleKey.D) {
                    buttons |= KPadButtons.Right;
                }
            }

            if (buttons != KPadButtons.None) {
                Console.WriteLine(buttons);
                mainBuffer = BitArrayHelper.FromByteLE((byte)buttons);
            }

            if (ChipSelect) {
                for (int i = 0; i < Size; i++) {
                    Outputs[i].Write(mainBuffer[i]);
                }
            }
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.AppendLine($"CS: {ChipSelect}, Buttons: {(KPadButtons)BitArrayHelper.ToByteLE(mainBuffer)}");

            return sb.ToString();
        }

        public void Dispose() {
            this.UnregisterUpdate();
        }
    }
}

using Components._Infrastructure.IODevices;
using Components.Signals;
using Infrastructure.BitArrays;
using Simulation.Updates;
using System.Collections;
using System.Text;
using System.Timers;

namespace ExternalDevices.Timers {
    public class KTimer : IODeviceBase, IExternalDevice, IUpdate {
        private readonly Timer timer;
        private readonly BitArray irrCode = new BitArray(4);

        public int Priority => -2;
        public SignalPort ChipSelect { get; set; }
        public SignalPort ExtIn { get; set; }
        public SignalPort ExtOut { get; set; }

        private SignalPort IrrrqOut => Outputs[0];
        private SignalPort RdyOut => Outputs[1];
        private SignalPort EnOut => Outputs[2];
        private SignalPort BusyOut => Outputs[3];

        private float frequency = 0;
        private KTimerStatus status;

        public KTimerStatus GetStatus() => status;
        public float GetFrequency() => frequency;
        public byte GetIrrCode() => new BitArray(4).MergeWith(irrCode).ToByteLE();

        private byte? lastInput = null;

        public KTimer(string name) : base(name) {
            // input is connected to data bus, output to irr register
            base.Initialize(8, 8);
            timer = new Timer();
        }

        void IExternalDevice.InitializeExternalDevice() {
            timer.Elapsed += Timer_Elapsed;
            this.RegisterUpdate();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e) {
            if (status == KTimerStatus.None) {
                status = KTimerStatus.Pending;
            }
        }

        private void WriteInterruptRequest() {
            IrrrqOut.Write(true);
            for (int i = 0; i < 4; i++) {
                Outputs[i + 4].Write(irrCode[i]);
            }
        }

        private void WriteClearInterrupt() {
            IrrrqOut.Write(false);
            for (int i = 0; i < 4; i++) {
                Outputs[i + 4].Write(false);
            }
        }

        private byte ReadInput() {
            var sum = 0;
            for (int i = 8 - 1; i >= 0; i--) {
                sum += Inputs[i] ? 1 << 8 - i - 1 : 0;
            }

            return (byte)sum;
        }

        private void ReadIrrCode(byte input) {
            for (int i = 0; i < 4; i++) {
                irrCode[i] = ((input >> i) & 1) == 1;
            }
        }

        private void InterpreteInput(byte input) {
            if (input == 0) {
                timer.Stop();
                frequency = 0;
            } else if (input >= 0xF0) {
                ReadIrrCode(input);
            } else {
                frequency = input;
                timer.Interval = 1f / frequency;
                timer.Start();
            }
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.AppendLine($"CS: {ChipSelect}, Frequency: {frequency}, IRRCode: {irrCode.ToBitString()}");

            return sb.ToString();
        }

        public void Update() {
            if (status == KTimerStatus.None) {
                var input = ReadInput();
                if (input != lastInput) {
                    lastInput = input;
                    InterpreteInput(input);
                }
            } else if (status == KTimerStatus.Pending && EnOut && !BusyOut) {
                WriteInterruptRequest();
                status = KTimerStatus.Requested;
            } else if (status == KTimerStatus.Requested && BusyOut) {
                status = KTimerStatus.InHandle;
            } else if (status == KTimerStatus.InHandle && RdyOut) {
                WriteClearInterrupt();
                status = KTimerStatus.None;
            }
        }

        public void Dispose() {
            this.UnregisterUpdate();
        }
    }

    public enum KTimerStatus {
        None,
        Pending,
        Requested,
        InHandle,
    }
}

using _Infrastructure.BitArrays;
using Components._Infrastructure.IODevices;
using Components.Signals;
using Infrastructure.BitArrays;
using Simulation.Updates;
using System.Text;

namespace Components.Transcievers {
    public class HLHiLoTransciever : IODeviceBase, IUpdate {
        public SignalPort OutputEnableLo { get; protected set; } = new SignalPort();
        public SignalPort OutputEnableHi { get; protected set; } = new SignalPort();

        private readonly int size;
        private readonly int halfSize;

        public HLHiLoTransciever(string name, int lanes) : base(name) {
            size = lanes;
            halfSize = lanes / 2;
            Initialize(lanes);
        }

        public void Initialize(int lanes) {
            base.Initialize(lanes, lanes);
            this.RegisterUpdate();
        }

        public void Update() {
            if (OutputEnableLo || OutputEnableHi) {
                WriteOutput();
            }
        }

        private void WriteOutput() {
            int start = 0;
            int end = size;

            if (OutputEnableLo && !OutputEnableHi) {
                start = halfSize;
            } else if (OutputEnableHi && !OutputEnableLo) {
                end = halfSize;
            }

            for (int i = start; i < end; i++) {
                Outputs[i].Write(Inputs[i]);
            }
        }

        public void Dispose() {
            this.UnregisterUpdate();
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.AppendLine($"OELo: {OutputEnableLo}, OEHi: {OutputEnableHi}, Outputs: {Outputs.ToBitArray().ToPrettyBitString()}");

            return sb.ToString();
        }
    }
}

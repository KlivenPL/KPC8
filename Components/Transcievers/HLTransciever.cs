using _Infrastructure.BitArrays;
using Components._Infrastructure.IODevices;
using Components.Signals;
using Infrastructure.BitArrays;
using _Infrastructure.Simulation.Updates;
using System.Text;

namespace Components.Transcievers {
    public class HLTransciever : IODeviceBase, ITransciever, IUpdate {
        public int Priority { get; set; } = 0;

        public SignalPort OutputEnable { get; protected set; } = new SignalPort();
        public HLTransciever(string name, int lanes) : base(name) {
            Initialize(lanes);
        }

        public void Initialize(int lanes) {
            base.Initialize(lanes, lanes);
            this.RegisterUpdate();
        }

        public void Update() {
            if (OutputEnable) {
                WriteOutput();
            }
        }

        private void WriteOutput() {
            for (int i = 0; i < Inputs.Length; i++) {
                Outputs[i].Write(Inputs[i]);
            }
        }

        public void Dispose() {
            this.UnregisterUpdate();
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.AppendLine($"OE: {(OutputEnable ? "1" : "0")}, Outputs: {Outputs.ToBitArray().ToPrettyBitString()}");

            return sb.ToString();
        }
    }
}

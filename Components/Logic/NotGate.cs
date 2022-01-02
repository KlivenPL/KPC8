using _Infrastructure.BitArrays;
using Components._Infrastructure.IODevices;
using Infrastructure.BitArrays;
using Simulation.Updates;
using System.Text;

namespace Components.Logic {
    public class NotGate : IODeviceBase, IUpdate {

        public NotGate(string name, int lanes) : base(name) {
            Initialize(lanes);
            this.RegisterUpdate();
        }

        public void Initialize(int lanes) {
            base.Initialize(lanes, lanes);
        }

        public void Update() {
            WriteOutput();
        }

        private void WriteOutput() {
            for (int i = 0; i < Inputs.Length; i++) {
                Outputs[i].Write(!Inputs[i]);
            }
        }

        public void Dispose() {
            this.UnregisterUpdate();
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.AppendLine($"Outputs: {Outputs.ToBitArray().ToPrettyBitString()}");

            return sb.ToString();
        }
    }
}

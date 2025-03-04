using _Infrastructure.BitArrays;
using Components._Infrastructure.IODevices;
using Components.Signals;
using Infrastructure.BitArrays;
using _Infrastructure.Simulation.Updates;
using System.Text;

namespace Components.Logic {
    public class SingleXorGate : IODeviceBase, IUpdate {
        public SignalPort Output => Outputs[0];

        public SingleXorGate(string name) : base(name) {
            Initialize(2);
            this.RegisterUpdate();
        }

        public void Initialize(int totalInputs) {
            base.Initialize(totalInputs, 1);
        }

        public void Update() {
            WriteOutput();
        }

        private void WriteOutput() {
            if (Inputs[0] && Inputs[1]) {
                Outputs[0].Write(false);
                return;
            }

            Outputs[0].Write(Inputs[0] || Inputs[1]);
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

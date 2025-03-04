using _Infrastructure.BitArrays;
using _Infrastructure.Simulation.Updates;
using Components._Infrastructure.IODevices;
using Components.Signals;
using Infrastructure.BitArrays;
using System.Linq;
using System.Text;

namespace Components.Logic {
    public class MultiAndGate : IODeviceBase, IUpdate {
        private readonly int outputsSize;

        public SignalPort[] InputA => Inputs.Take(outputsSize).ToArray();
        public SignalPort[] InputB => Inputs.TakeLast(outputsSize).ToArray();

        public MultiAndGate(string name, int totalInputs) : base(name) {
            if (totalInputs % 2 != 0) {
                throw new System.Exception("Total inputs must be even.");
            }

            outputsSize = totalInputs / 2;
            Initialize(totalInputs);
            this.RegisterUpdate();
        }

        public void Initialize(int totalInputs) {
            base.Initialize(totalInputs, outputsSize);
        }

        public void Update() {
            WriteOutput();
        }

        private void WriteOutput() {
            for (int i = 0; i < outputsSize; i++) {
                Outputs[i].Write(Inputs[i] & Inputs[outputsSize + i]);
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

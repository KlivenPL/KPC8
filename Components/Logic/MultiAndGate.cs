using Components._Infrastructure.IODevices;
using Components.Signals;
using Simulation.Updates;
using System.Linq;

namespace Components.Logic {
    public class MultiAndGate : IODeviceBase, IUpdate {
        private readonly int outputsSize;

        public SignalPort[] InputA => Inputs.Take(outputsSize).ToArray();
        public SignalPort[] InputB => Inputs.TakeLast(outputsSize).ToArray();

        public MultiAndGate(int totalInputs) {
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
            for (int i = 0; i < outputsSize / 2; i++) {
                Outputs[i].Write(Inputs[i] && Inputs[outputsSize - i]);
            }
        }

        public void Dispose() {
            this.UnregisterUpdate();
        }
    }
}

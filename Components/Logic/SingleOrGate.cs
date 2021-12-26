using Components._Infrastructure.IODevices;
using Components.Signals;
using Simulation.Updates;

namespace Components.Logic {
    public class SingleOrGate : IODeviceBase, IUpdate {
        public SignalPort Output => Outputs[0];

        public SingleOrGate(int totalInputs) {
            if (totalInputs < 2) {
                throw new System.Exception("At least 2 inputs are required");
            }

            Initialize(totalInputs);
            this.RegisterUpdate();
        }

        public void Initialize(int totalInputs) {
            base.Initialize(totalInputs, 1);
        }

        public void Update() {
            WriteOutput();
        }

        private void WriteOutput() {
            for (int i = 0; i < Inputs.Length; i++) {
                if (Inputs[i]) {
                    Outputs[0].Write(true);
                    return;
                }
            }

            Outputs[0].Write(false);
        }

        public void Dispose() {
            this.UnregisterUpdate();
        }
    }
}

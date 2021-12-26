using Components._Infrastructure.IODevices;
using Simulation.Updates;

namespace Components.Logic {
    public class Inverter : IODeviceBase, IUpdate {

        public Inverter(int lanes) {
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
    }
}

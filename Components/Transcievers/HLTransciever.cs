using Components._Infrastructure.IODevices;
using Components.Signals;
using Simulation.Updates;

namespace Components.Transcievers {
    public class HLTransciever : IODeviceBase, ITransciever, IUpdate {
        public SignalPort OutputEnable { get; protected set; } = new SignalPort();
        public HLTransciever(int lanes) {
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
    }
}

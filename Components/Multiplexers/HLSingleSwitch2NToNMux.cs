using Components._Infrastructure.IODevices;
using Components.Signals;
using Simulation.Updates;
using System.Linq;

namespace Components.Multiplexers {
    public class HLSingleSwitch2NToNMux : IODeviceBase, IMux, IUpdate {
        public SignalPort SelectB { get; protected set; } = new SignalPort();
        public SignalPort[] InputsA => Inputs.Take(singleInputSize).ToArray();
        public SignalPort[] InputsB => Inputs.Skip(singleInputSize).ToArray();

        private readonly int singleInputSize;

        public HLSingleSwitch2NToNMux(int singleInputSize) {
            this.singleInputSize = singleInputSize;

            Initialize();
            this.RegisterUpdate();
        }

        public void Initialize() {
            base.Initialize(singleInputSize * 2, singleInputSize);
        }

        public void Update() {
            WriteOutput();
        }

        private void WriteOutput() {
            var inputs = SelectB ? InputsB : InputsA;

            for (int i = 0; i < Outputs.Length; i++) {
                Outputs[i].Write(inputs[i]);
            }
        }

        public void Dispose() {
            this.UnregisterUpdate();
        }
    }
}

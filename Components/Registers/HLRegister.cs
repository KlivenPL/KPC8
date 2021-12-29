using Components._Infrastructure.IODevices;
using Components.Signals;
using Simulation.Updates;
using System.Collections;

namespace Components.Registers {
    public class HLRegister : IODeviceBase, IRegister, IUpdate {
        protected readonly BitArray mainBuffer;

        public SignalPort LoadEnable { get; protected set; } = new SignalPort();
        public SignalPort OutputEnable { get; protected set; } = new SignalPort();
        public SignalPort Clear { get; protected set; } = new SignalPort();
        public SignalPort Clk { get; protected set; } = new SignalPort();

        public BitArray Content => new(mainBuffer);

        public HLRegister(int size) {
            mainBuffer = new(size);
            Initialize(size);
            this.RegisterUpdate();
        }

        public void Initialize(int size) {
            base.Initialize(size, size);

            Clk.OnEdgeRise += Clk_OnEdgeRise;
        }

        public virtual void Update() {
            if (OutputEnable) {
                WriteOutput();
            }
        }

        protected virtual void Clk_OnEdgeRise() {
            if (Clear) {
                mainBuffer.SetAll(false);
            }

            if (LoadEnable) {
                LoadInput();
            }
        }

        protected virtual void LoadInput() {
            for (int i = 0; i < Inputs.Length; i++) {
                mainBuffer[i] = Inputs[i];
            }
        }

        private void WriteOutput() {
            for (int i = 0; i < Inputs.Length; i++) {
                Outputs[i].Write(mainBuffer[i]);
            }
        }

        public void Dispose() {
            this.UnregisterUpdate();
        }
    }
}

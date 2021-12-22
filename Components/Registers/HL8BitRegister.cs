using Components._Infrastructure.IODevices;
using Components.Signals;
using Simulation.Updates;
using System.Collections;

namespace Components.Registers {
    public class HL8BitRegister : IODeviceBase, IRegister, IUpdate {
        private readonly BitArray mainBuffer = new(8);

        public SignalPort LoadEnable { get; protected set; } = new SignalPort();
        public SignalPort OutputEnable { get; protected set; } = new SignalPort();
        public SignalPort Clear { get; protected set; } = new SignalPort();
        public SignalPort Clk { get; protected set; } = new SignalPort();

        public BitArray Content => new(mainBuffer);

        public HL8BitRegister() {
            Initialize();
        }

        public void Initialize() {
            base.Initialize(8, 8);

            Clk.OnEdgeRise += Clk_OnEdgeRise;
            Clear.OnEdgeRise += Clear_OnEdgeRise;

            this.RegisterUpdate();
        }

        public void Update() {
            if (OutputEnable) {
                WriteOutput();
            }
        }

        private void Clear_OnEdgeRise() {
            mainBuffer.SetAll(false);
        }

        private void Clk_OnEdgeRise() {
            if (LoadEnable) {
                LoadInput();
            }
        }

        private void LoadInput() {
            for (int i = 0; i < 8; i++) {
                mainBuffer[i] = Inputs[i];
            }
        }

        private void WriteOutput() {
            for (int i = 0; i < 8; i++) {
                Outputs[i].Write(mainBuffer[i]);
            }
        }

        public void Dispose() {
            this.UnregisterUpdate();
        }
    }
}

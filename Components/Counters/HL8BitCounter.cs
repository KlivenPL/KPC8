using Components._Infrastructure.IODevices;
using Components.Signals;
using Simulation.Updates;
using System.Collections;

namespace Components.Counters {
    public class HL8BitCounter : IODeviceBase, ICounter, IUpdate {
        private readonly BitArray mainBuffer = new(8);

        public SignalPort LoadEnable { get; protected set; } = new SignalPort();
        public SignalPort OutputEnable { get; protected set; } = new SignalPort();
        public SignalPort CountEnable { get; protected set; } = new SignalPort();
        public SignalPort Clear { get; protected set; } = new SignalPort();
        public SignalPort Clk { get; protected set; } = new SignalPort();

        public BitArray Content => new(mainBuffer);

        public HL8BitCounter() {
            Initialize();
        }

        public void Initialize() {
            base.Initialize(8, 8);

            Clk.OnEdgeRise += Clk_OnEdgeRise;
            Clear.OnEdgeRise += Clear_OnEdgeRise;

            this.RegisterUpdate();
        }

        public void Update() {
            if (LoadEnable) {
                LoadInput();
            }

            if (OutputEnable) {
                WriteOutput();
            }
        }

        private void Clear_OnEdgeRise() {
            mainBuffer.SetAll(false);
        }

        private void Clk_OnEdgeRise() {
            if (CountEnable) {
                Count();
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

        private void Count() {
            var carryIn = true;

            for (int i = 7; i >= 0; i--) {
                var tmp = mainBuffer[i] ^ carryIn;
                carryIn = mainBuffer[i] && carryIn;
                mainBuffer[i] = tmp;
            }
        }

        public void Dispose() {
            this.UnregisterUpdate();
        }
    }
}

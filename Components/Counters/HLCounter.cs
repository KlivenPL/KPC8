using Components._Infrastructure.IODevices;
using Components.Signals;
using Simulation.Updates;
using System.Collections;

namespace Components.Counters {
    public class HLCounter : IODeviceBase, ICounter, IUpdate {
        protected readonly BitArray mainBuffer;

        public SignalPort LoadEnable { get; protected set; } = new SignalPort();
        public SignalPort OutputEnable { get; protected set; } = new SignalPort();
        public SignalPort CountEnable { get; protected set; } = new SignalPort();
        public SignalPort Clear { get; protected set; } = new SignalPort();
        public SignalPort Clk { get; protected set; } = new SignalPort();

        public BitArray Content => new(mainBuffer);

        public HLCounter(int size) {
            mainBuffer = new(size);
            Initialize(size);
        }

        public void Initialize(int size) {
            base.Initialize(size, size);

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

            if (CountEnable) {
                Count();
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

        private void Count() {
            var carryIn = true;

            for (int i = Inputs.Length - 1; i >= 0; i--) {
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

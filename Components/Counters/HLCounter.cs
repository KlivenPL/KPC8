using Components._Infrastructure.IODevices;
using Components.Signals;
using Infrastructure.BitArrays;
using Simulation.Updates;
using System.Collections;
using System.Text;

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
            this.RegisterUpdate();
        }

        public void Initialize(int size) {
            base.Initialize(size, size);

            Clk.OnEdgeRise += Clk_OnEdgeRise;
        }

        public void Update() {
            if (OutputEnable) {
                WriteOutput();
            }
        }

        private void Clk_OnEdgeRise() {
            if (Clear) {
                mainBuffer.SetAll(false);
            } else if (CountEnable) {
                Count();
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

        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine($"Content: {Content.ToPrettyBitString()}");

            return sb.ToString();
        }
    }
}

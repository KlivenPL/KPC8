using Components._Infrastructure.IODevices;
using Components.Signals;
using Infrastructure.BitArrays;
using Simulation.Updates;
using System.Collections;
using System.Text;

namespace Components.Registers {
    public class HLRegister : IODeviceBase, IRegister, IUpdate {
        protected readonly BitArray mainBuffer;

        public int Priority { get; set; } = -1;
        public SignalPort LoadEnable { get; protected set; } = new SignalPort();
        public SignalPort OutputEnable { get; protected set; } = new SignalPort();
        public SignalPort Clear { get; protected set; } = new SignalPort();
        public SignalPort Clk { get; protected set; } = new SignalPort();

        public BitArray Content => new(mainBuffer);

        public void SetContent(BitArray value) {
            for (int i = 0; i < mainBuffer.Length; i++) {
                mainBuffer[i] = value[i];
            }
        }

        private bool clkEdgeRise = false;

        public HLRegister(string name, int size) : base(name) {
            mainBuffer = new(size);
            Initialize(size);
            this.RegisterUpdate();
        }

        public void Initialize(int size) {
            base.Initialize(size, size);

            Clk.OnEdgeRise += Clk_OnEdgeRise;
        }

        public virtual void Update() {
            if (clkEdgeRise) {
                if (Clear) {
                    mainBuffer.SetAll(false);
                }

                if (LoadEnable) {
                    LoadInput();
                }

                clkEdgeRise = false;
            }

            if (OutputEnable && Clk) {
                WriteOutput();
            }
        }

        protected virtual void Clk_OnEdgeRise() {
            clkEdgeRise = true;
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

        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.AppendLine($"LE: {(LoadEnable ? "1" : "0")}, Content: {Content.ToPrettyBitString()}");

            return sb.ToString();
        }
    }
}

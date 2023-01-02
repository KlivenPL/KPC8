using Components._Infrastructure.IODevices;
using Components.Signals;
using Infrastructure.BitArrays;
using _Infrastructure.Simulation.Updates;
using System.Collections;
using System.Text;

namespace Components.Registers {
    public class HLLohRegister : IODeviceBase, IUpdate {
        protected readonly BitArray mainBuffer;

        public int Priority => -1;
        public SignalPort L { get; protected set; } = new SignalPort();
        public SignalPort O { get; protected set; } = new SignalPort();
        public SignalPort H { get; protected set; } = new SignalPort();
        public SignalPort Clk { get; protected set; } = new SignalPort();
        public SignalPort ChipEnable { get; protected set; } = new SignalPort();

        public BitArray Content => new(mainBuffer);
        public void SetContent(BitArray value) {
            for (int i = 0; i < mainBuffer.Length; i++) {
                mainBuffer[i] = value[i];
            }
        }

        private bool clkEdgeRise = false;
        private readonly int size;
        private readonly int halfSize;

        public HLLohRegister(string name, int size) : base(name) {
            mainBuffer = new(size);
            this.size = size;
            halfSize = size / 2;
            Initialize(size);
            this.RegisterUpdate();
        }

        public void Initialize(int size) {
            base.Initialize(size, size);

            Clk.OnEdgeRise += Clk_OnEdgeRise;
        }

        public virtual void Update() {
            if (!ChipEnable) {
                return;
            }

            if (clkEdgeRise) {
                if (L && O && H) {
                    mainBuffer.SetAll(false);
                }

                if (L && !O && !H) {
                    LoadLower();
                } else if (L && !O && H) {
                    LoadHigher();
                } else if (L && O && !H) {
                    LoadWhole();
                }

                clkEdgeRise = false;
            }

            if (!L && O && !H) {
                OutputLower();
            } else if (!L && O && H) {
                OutputHigher();
            } else if (!L && !O && H) {
                OutputWhole();
            }
        }

        protected virtual void Clk_OnEdgeRise() {
            clkEdgeRise = true;
        }
        private void LoadLower() {
            for (int i = halfSize; i < size; i++) {
                mainBuffer[i] = Inputs[i];
            }
        }

        private void LoadHigher() {
            for (int i = 0; i < halfSize; i++) {
                mainBuffer[i] = Inputs[i];
            }
        }

        private void LoadWhole() {
            for (int i = 0; i < size; i++) {
                mainBuffer[i] = Inputs[i];
            }
        }

        private void OutputLower() {
            for (int i = halfSize; i < size; i++) {
                Outputs[i].Write(mainBuffer[i]);
            }
        }

        private void OutputHigher() {
            for (int i = 0; i < halfSize; i++) {
                Outputs[i].Write(mainBuffer[i]);
            }
        }

        private void OutputWhole() {
            for (int i = 0; i < size; i++) {
                Outputs[i].Write(mainBuffer[i]);
            }
        }

        public void Dispose() {
            this.UnregisterUpdate();
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.AppendLine($"LE: {(L ? "1" : "0")}, Content: {Content.ToPrettyBitString()} ({BitArrayHelper.ToShortLE(Content)})");

            return sb.ToString();
        }
    }
}

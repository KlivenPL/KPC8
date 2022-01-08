using Components._Infrastructure.IODevices;
using Components.Signals;
using Infrastructure.BitArrays;
using Simulation.Updates;
using System.Collections;
using System.Text;

namespace Components.Sequencers {
    public class HLIRRSequencer : IODeviceBase, IUpdate {
        protected readonly BitArray mainBuffer;

        public int Priority => -1;
        public SignalPort IRRRQ { get; protected set; } = new SignalPort(); // gets from HW
        public Signal RDY { get; protected set; } = Signal.Factory.Create("IRR_RDY"); // controls by itself
        public Signal EN { get; protected set; } = Signal.Factory.Create("IRR_EN"); // controls by itself
        public Signal BUSY { get; protected set; } = Signal.Factory.Create("IRR_BUSY"); // controls by itself

        public SignalPort Irr_a { get; protected set; } = new SignalPort(); // gets from CPU
        public SignalPort Irr_b { get; protected set; } = new SignalPort(); // gets from CPU
        public SignalPort Ir_clr { get; protected set; } = new SignalPort(); // gets from CPU

        public SignalPort ShouldProcessInterrupt { get; protected set; } = new SignalPort();
        public SignalPort MainClock { get; protected set; } = new SignalPort();

        public BitArray IrrCode => new(mainBuffer);

        private bool clkEdgeRise = false;
        private bool isBusyServingInterrupt = false;
        private bool deviceAbort = false;
        private const int Size = 4;

        public HLIRRSequencer(string name) : base(name) {
            mainBuffer = new(Size);
            Initialize(Size);
            this.RegisterUpdate();
        }

        public void Initialize(int size) {
            base.Initialize(size, size);

            MainClock.OnEdgeRise += Clk_OnEdgeRise;
            IRRRQ.OnEdgeFall += IRRRQ_OnEdgeFall;
        }

        private void IRRRQ_OnEdgeFall() {
            if (isBusyServingInterrupt) {
                deviceAbort = true;
            } else {
                RDY.Value = false;
            }
        }

        public virtual void Update() {
            if (clkEdgeRise) {
                LoadWhole();

                if (!Irr_a && Irr_b) {
                    EN.Value = true;
                } else if (Irr_a && !Irr_b) {
                    EN.Value = false;
                } else if (Irr_a && Irr_b) {
                    isBusyServingInterrupt = !isBusyServingInterrupt;
                    if (!isBusyServingInterrupt) {
                        if (deviceAbort) {
                            deviceAbort = false;
                            RDY.Value = false;
                        } else {
                            RDY.Value = true;
                        }
                    }
                }

                clkEdgeRise = false;
            }

            var shouldProcessInterrupt = !isBusyServingInterrupt && Ir_clr && IRRRQ && !RDY && EN;
            if (shouldProcessInterrupt) {
                OutputWhole();
            }

            ShouldProcessInterrupt.Write(shouldProcessInterrupt);
            BUSY.Value = shouldProcessInterrupt || isBusyServingInterrupt || deviceAbort || RDY;
        }

        protected virtual void Clk_OnEdgeRise() {
            clkEdgeRise = true;
        }

        private void LoadWhole() {
            for (int i = 0; i < Size; i++) {
                mainBuffer[i] = Inputs[i];
            }
        }

        private void OutputWhole() {
            for (int i = 0; i < Size; i++) {
                Outputs[i].Write(mainBuffer[i]);
            }
        }

        public void Dispose() {
            this.UnregisterUpdate();
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.AppendLine($"IRRRQ: {IRRRQ}, Irr_rdy: {RDY}, Irr_en: {EN}, Ir_clr: {Ir_clr}, Irr code: {IrrCode.ToPrettyBitString()}");

            return sb.ToString();
        }
    }
}

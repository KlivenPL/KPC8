using Components._Infrastructure.IODevices;
using Components.Signals;
using Infrastructure.BitArrays;
using Simulation.Updates;
using System.Collections;
using System.Text;

namespace Components.Sequencers {
    public class HLIRRSequencer : IODeviceBase, IUpdate {
        private enum IRRSequencerState {
            None,
            IrPending,
            IcCleared,
            IrInHandle,
            IrFinished,
            IrAborted,
        }

        protected readonly BitArray mainBuffer;

        public int Priority { get; set; }
        public SignalPort IRRRQ { get; protected set; } = new SignalPort(); // gets from HW
        public SignalPort RDY { get; protected set; } = new SignalPort(); // controls by itself
        public SignalPort EN { get; protected set; } = new SignalPort(); // controls by itself
        public SignalPort BUSY { get; protected set; } = new SignalPort(); // controls by itself

        public SignalPort Irr_a { get; protected set; } = new SignalPort(); // gets from CPU
        public SignalPort Irr_b { get; protected set; } = new SignalPort(); // gets from CPU

        public SignalPort Ic_clr { get; protected set; } = new SignalPort(); // gets from CPU
        public SignalPort Ir_le { get; protected set; } = new SignalPort(); // gets from CPU

        public SignalPort ShouldProcessInterrupt { get; protected set; } = new SignalPort();
        public SignalPort MainClockBar { get; protected set; } = new SignalPort();

        public BitArray IrrCode => new(mainBuffer);

        private bool clkEdgeRise = false;
        private const int Size = 4;
        private IRRSequencerState state;

        public HLIRRSequencer(string name) : base(name) {
            mainBuffer = new(Size);
            Initialize(Size);
            this.RegisterUpdate();
        }

        public void Initialize(int size) {
            base.Initialize(size, size);

            MainClockBar.OnEdgeRise += Clk_OnEdgeRise;
            IRRRQ.OnEdgeRise += IRRRQ_OnEdgeRise;
            IRRRQ.OnEdgeFall += IRRRQ_OnEdgeFall;
            Ic_clr.OnEdgeRise += Ic_clr_OnEdgeRise;
        }

        private void Ic_clr_OnEdgeRise() {
            if (state == IRRSequencerState.IrPending && EN) {
                state = IRRSequencerState.IcCleared;
            }
        }

        private void IRRRQ_OnEdgeRise() {
            switch (state) {
                case IRRSequencerState.None:
                    state = IRRSequencerState.IrPending;
                    break;
            }
        }

        private void IRRRQ_OnEdgeFall() {
            switch (state) {
                case IRRSequencerState.None:
                    break;
                case IRRSequencerState.IrPending:
                    state = IRRSequencerState.None;
                    break;
                case IRRSequencerState.IcCleared:
                    break;
                case IRRSequencerState.IrInHandle:
                    state = IRRSequencerState.IrAborted;
                    break;
                case IRRSequencerState.IrFinished:
                    state = IRRSequencerState.None;
                    break;
                case IRRSequencerState.IrAborted:
                    throw new System.Exception("Invalid state on irrq edge fall");
            }
        }

        public virtual void Update() {
            if (clkEdgeRise) {
                clkEdgeRise = false;

                LoadWhole();

                bool irr_a = Irr_a, irr_b = Irr_b;

                if (!irr_a && irr_b) {
                    EN.Write(true);
                } else if (irr_a && !irr_b) {
                    EN.Write(false);
                } else if (irr_a && irr_b) {

                    switch (state) {
                        case IRRSequencerState.IcCleared:
                            state = IRRSequencerState.IrInHandle;
                            break;
                        case IRRSequencerState.IrInHandle:
                            state = IRRSequencerState.IrFinished;
                            break;
                        case IRRSequencerState.IrAborted:
                            if (IRRRQ) {
                                state = IRRSequencerState.IrPending;
                            } else {
                                state = IRRSequencerState.None;
                            }
                            break;
                        default:
                            throw new System.Exception("Invalid state");
                    }
                }
            }

            if (state == IRRSequencerState.IcCleared && Ir_le) {
                OutputWhole();
                ShouldProcessInterrupt.Write(true);
            } else {
                ShouldProcessInterrupt.Write(false);
            }

            RDY.Write(state == IRRSequencerState.IrFinished);
            BUSY.Write(state != IRRSequencerState.None);

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
            sb.AppendLine($"IRRRQ: {IRRRQ}, Irr_rdy: {RDY}, Irr_en: {EN}, Ir_le: {Ir_le}, State: {state}, Irr code: {IrrCode.ToPrettyBitString()}");

            return sb.ToString();
        }
    }
}

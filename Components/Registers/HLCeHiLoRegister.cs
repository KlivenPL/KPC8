using Components.Signals;

namespace Components.Registers {
    public class HLCeHiLoRegister : HLHiLoRegister {
        public SignalPort ChipEnable { get; protected set; } = new SignalPort();

        public HLCeHiLoRegister(string name, int size) : base(name, size) { }

        public override void Update() {
            if (ChipEnable) {
                base.Update();
            }
        }

        protected override void Clk_OnEdgeRise() {
            if (ChipEnable) {
                base.Clk_OnEdgeRise();
            }
        }
    }
}

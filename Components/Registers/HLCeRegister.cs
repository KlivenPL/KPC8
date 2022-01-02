using Components.Signals;

namespace Components.Registers {
    public class HLCeRegister : HLRegister {
        public SignalPort ChipEnable { get; protected set; } = new SignalPort();

        public HLCeRegister(string name, int size) : base(name, size) { }

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

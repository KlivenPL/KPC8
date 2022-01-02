using Components.Signals;

namespace Components.Registers {
    public class HLHiLoRegister : HLRegister {
        public SignalPort LoadEnableHigh { get; protected set; } = new SignalPort();
        public SignalPort LoadEnableLow { get; protected set; } = new SignalPort();

        public HLHiLoRegister(string name, int size) : base(name, size) { }

        protected override void LoadInput() {
            int start = 0;
            int end = Inputs.Length;

            if (LoadEnableLow && !LoadEnableHigh) {
                start = end / 2;
            } else if (LoadEnableHigh && !LoadEnableLow) {
                end /= 2;
            }

            for (int i = start; i < end; i++) {
                mainBuffer[i] = Inputs[i];
            }
        }
    }
}

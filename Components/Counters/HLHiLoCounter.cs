using Components.Signals;

namespace Components.Counters {
    public class HLHiLoCounter : HLCounter {
        public SignalPort LoadEnableHigh { get; protected set; } = new SignalPort();
        public SignalPort LoadEnableLow { get; protected set; } = new SignalPort();

        public HLHiLoCounter(int size) : base(size) { }

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

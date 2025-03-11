using Components.Signals;
using Infrastructure.BitArrays;
using System.Collections;

namespace Components.Counters {
    public class HLHiLoCounter : HLCounter, Abstract.Components.IRegister16 {
        public SignalPort LoadEnableHigh { get; protected set; } = new SignalPort();
        public SignalPort LoadEnableLow { get; protected set; } = new SignalPort();

        public HLHiLoCounter(string name, int size) : base(name, size) { }

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

        public byte HighValue {
            get {
                uint combined = mainBuffer.ToUShortLE();
                return (byte)(combined >> halfSize);
            }

            set => SetContent(BitArrayHelper.FromByteLE(value).MergeWith(mainBuffer.Skip(halfSize)));
        }

        public byte LowValue {
            get {
                uint combined = mainBuffer.ToUShortLE();
                return (byte)(combined << halfSize);
            }

            set => SetContent(mainBuffer.Take(halfSize).MergeWith(BitArrayHelper.FromByteLE(value)));
        }

        public ushort WordValue {
            get => mainBuffer.ToUShortLE();
            set => SetContent(BitArrayHelper.FromUShortLE(value));
        }

        private void SetContent(BitArray value) {
            for (int i = 0; i < mainBuffer.Length; i++) {
                mainBuffer[i] = value[i];
            }
        }
    }
}

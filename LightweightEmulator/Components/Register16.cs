using Abstract.Components;

namespace LightweightEmulator.Components {
    public class Register16 : IRegister16 {
        public Register16() { }

        public Register16(int wordValue) {
            WordValue = (ushort)wordValue;
        }

        public virtual byte HighValue { get; set; }

        public virtual byte LowValue { get; set; }

        public virtual ushort WordValue {
            get => (ushort)((HighValue << 8) | LowValue);

            set {
                HighValue = (byte)((value & 0xFF00) >> 8);
                LowValue = (byte)(value & 0x00FF);
            }
        }

        public Register16 Clone() {
            return new Register16 {
                WordValue = WordValue,
            };
        }
    }

    public class ZeroRegister : Register16 {
        public override byte HighValue {
            get => 0;
            set { }
        }

        public override byte LowValue {
            get => 0;
            set { }
        }

        public override ushort WordValue {
            get => 0;

            set { }
        }
    }
}

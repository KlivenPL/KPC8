namespace LightweightEmulator.Components {
    public class Register16 {

        public Register16() { }

        public Register16(int wordValue) {
            WordValue = (ushort)wordValue;
        }

        public byte HighValue { get; set; }

        public byte LowValue { get; set; }

        public ushort WordValue {
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
}

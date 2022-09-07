namespace LightweightEmulator.Components {
    internal class Register {
        public byte HiValue { get; set; }

        public byte LowValue { get; set; }

        public ushort WordValue {
            get => (ushort)((HiValue << 8) | LowValue);

            set {
                HiValue = (byte)((value & 0xFF00) >> 8);
                LowValue = (byte)(value & 0x00FF);
            }
        }
    }
}

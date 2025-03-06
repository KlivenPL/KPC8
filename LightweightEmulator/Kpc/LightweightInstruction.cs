namespace LightweightEmulator.Kpc {
    internal class LightweightInstruction {
        public LightweightInstruction(byte instructionHigh, byte instructionLow) {
            Type = (KpcInstructionType)(ushort)(instructionHigh >> 2);
            RegDestIndex = (instructionHigh & 0x03) + 4;
            RegAIndex = (instructionLow & 0xF0) >> 4;
            RegBIndex = instructionLow & 0x0F;
            ImmediateValue = instructionLow;
        }

        public KpcInstructionType Type { get; }
        public int RegDestIndex { get; }
        public int RegAIndex { get; }
        public int RegBIndex { get; }
        public byte ImmediateValue { get; }
    }
}

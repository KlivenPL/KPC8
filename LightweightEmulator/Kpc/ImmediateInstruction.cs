namespace LightweightEmulator.Kpc {
    internal class ImmediateInstruction {
        public ImmediateInstruction(byte instructionHigh, byte instructionLow) {
            Type = (KpcInstructionType)(ushort)(instructionHigh >> 2);
            RegDestIndex = (instructionHigh & 0x03) + 4;
            ImmediateValue = instructionLow;
        }

        public KpcInstructionType Type { get; }
        public int RegDestIndex { get; }
        public byte ImmediateValue { get; }
    }
}

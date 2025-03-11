namespace Abstract.Components {
    public interface IMemory {
        void WriteByte(byte data, ushort address);
        void WriteWord(ushort data, ushort address);
        byte ReadByte(ushort address);
        ushort ReadWord(ushort address);
        byte[] DumpToBytes();
    }
}

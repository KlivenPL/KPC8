using Abstract.Components;

namespace LightweightEmulator.Components {
    public class Memory : IMemory {
        private readonly byte[] storage;

        public Memory(int size) {
            storage = new byte[size];
        }

        public Memory(int size, byte[]? bytes) {
            storage = new byte[size];
            bytes?.CopyTo(storage, 0);
        }

        public void WriteByte(byte data, ushort address) {
            storage[address] = data;
        }

        public void WriteWord(ushort data, ushort address) {
            storage[address] = (byte)((data & 0xFF00) >> 8);
            storage[address + 1] = (byte)(data & 0x00FF);
        }

        public byte ReadByte(ushort address) {
            return storage[address];
        }

        public ushort ReadWord(ushort address) {
            var b1 = storage[address];
            var b2 = storage[address + 1];

            return (ushort)(b1 << 8 | b2);
        }

        public byte[] DumpToBytes() {
            var bytes = new byte[storage.Length];
            storage.CopyTo(bytes, 0);
            return bytes;
        }
    }
}

using _Infrastructure.BitArrays;
using _Infrastructure.Simulation.Updates;
using Components._Infrastructure.IODevices;
using Components.Signals;
using Infrastructure.BitArrays;
using System.Collections;
using System.Linq;
using System.Text;

namespace Components.Rams {
    public class HLRam : IODeviceBase, IRam, IUpdate {
        private readonly int MemorySizeInBytes;
        private readonly int AddressSize;
        private readonly int DataSize;

        private readonly BitArray[] memory;

        public int Priority => -2;
        public BitArray[] Content => memory;
        public SignalPort WriteEnable { get; protected set; } = new SignalPort();
        public SignalPort OutputEnable { get; protected set; } = new SignalPort();
        public SignalPort Clk { get; protected set; } = new SignalPort();

        public SignalPort[] AddressInputs => Inputs.Take(AddressSize).ToArray();
        public SignalPort[] DataInputs => Inputs.Skip(AddressSize).ToArray();

        private bool clkEdgeRise = false;

        public HLRam(string name, int dataSize, int addressSize, int totalSizeInBytes, BitArray[] initialMemory = null) : base(name) {
            MemorySizeInBytes = totalSizeInBytes;
            AddressSize = addressSize;
            DataSize = dataSize;
            memory = new BitArray[MemorySizeInBytes];
            Initialize(initialMemory);
        }

        public void Initialize(BitArray[] initialMemory) {
            base.Initialize(AddressSize + DataSize, DataSize);

            if (initialMemory == null) {
                for (int i = 0; i < MemorySizeInBytes; i++) {
                    memory[i] = new BitArray(DataSize);
                }
            } else {
                for (int i = 0; i < initialMemory.Length; i++) {
                    memory[i] = new BitArray(initialMemory[i] ?? new BitArray(DataSize));
                }
                if (initialMemory.Length < MemorySizeInBytes) {
                    for (int i = initialMemory.Length; i < MemorySizeInBytes; i++) {
                        memory[i] = new BitArray(DataSize);
                    }
                }
            }

            Clk.OnEdgeRise += Clk_OnEdgeRise;
            this.RegisterUpdate();
        }

        public void Update() {
            if (clkEdgeRise) {
                if (WriteEnable) {
                    WriteData();
                }
                clkEdgeRise = false;
            }

            if (OutputEnable) {
                WriteOutput();
            }
        }

        private void Clk_OnEdgeRise() {
            clkEdgeRise = true;
        }

        private void WriteData() {
            var addr = GetMemoryAddress();
            for (int i = 0; i < DataSize; i++) {
                memory[addr][i] = Inputs[i + AddressSize];
            }
        }

        private void WriteOutput() {
            var addr = GetMemoryAddress();
            for (int i = 0; i < DataSize; i++) {
                Outputs[i].Write(memory[addr][i]);
            }
        }

        private int GetMemoryAddress() {
            var sum = 0;
            for (int i = AddressSize - 1; i >= 0; i--) {
                sum += Inputs[i] ? 1 << AddressSize - i - 1 : 0;
            }

            return sum;
        }

        public void Dispose() {
            this.UnregisterUpdate();
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.AppendLine($"AddressInputs: {AddressInputs.ToBitArray().ToPrettyBitString()}");
            sb.AppendLine($"DataInputs: {DataInputs.ToBitArray().ToPrettyBitString()}");
            sb.AppendLine($"Content@Addr: {memory[GetMemoryAddress()].ToPrettyBitString()}");

            return sb.ToString();
        }

        public void WriteByte(byte data, ushort address) {
            memory[address] = BitArrayHelper.FromByteLE(data);
        }

        public void WriteWord(ushort data, ushort address) {
            memory[address] = BitArrayHelper.FromByteLE((byte)((data & 0xFF00) >> 8));
            memory[address + 1] = BitArrayHelper.FromByteLE((byte)(data & 0x00FF));
        }

        public byte ReadByte(ushort address) {
            return memory[address].ToByteLE();
        }

        public ushort ReadWord(ushort address) {
            var b1 = memory[address].ToByteLE();
            var b2 = memory[address + 1].ToByteLE();

            return (ushort)(b1 << 8 | b2);
        }

        public byte[] DumpToBytes() {
            var bytes = new byte[memory.Length];
            memory.CopyTo(bytes, 0);
            return bytes;
        }
    }
}

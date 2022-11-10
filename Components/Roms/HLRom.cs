using _Infrastructure.BitArrays;
using Components._Infrastructure.IODevices;
using Components.Signals;
using Infrastructure.BitArrays;
using Simulation.Updates;
using System.Collections;
using System.Linq;
using System.Text;

namespace Components.Roms {
    public class HLRom : IODeviceBase, IRom, IUpdate {
        public int Priority { get; set; } = -2;

        private readonly int MemorySizeInBytes;
        private readonly int AddressSize;
        private readonly int DataSize;

        private readonly BitArray[] memory;

        public BitArray[] Content => memory;
        public SignalPort OutputEnable { get; protected set; } = new SignalPort();

        public SignalPort[] AddressInputs => Inputs.Take(AddressSize).ToArray();

        public HLRom(string name, int dataSize, int addressSize, int totalSizeInBytes, BitArray[] initialMemory) : base(name) {
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

            this.RegisterUpdate();
        }

        public void Update() {
            if (OutputEnable) {
                WriteOutput();
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
            sb.AppendLine($"Address: {AddressInputs.ToBitArray().ToPrettyBitString()}");
            sb.AppendLine($"Content@Addr: {memory[GetMemoryAddress()].ToPrettyBitString()}");

            return sb.ToString();
        }
    }
}

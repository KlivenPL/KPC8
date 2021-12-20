using Components.IODevices;
using Components.Signals;
using Infrastructure.BitArrays;
using Simulation.Updates;
using System.Collections;

namespace Components.Rams {
    class HL256Ram : IODeviceBase, IRam, IUpdate {
        private const int MemorySizeInBytes = 256; // 2^AddressSize
        private const int AddressSize = 8;
        private const int DataSize = 8;
        private const int OutputSize = 8;

        private readonly BitArray[] memory = new BitArray[MemorySizeInBytes];

        public BitArray[] Content => memory;
        public SignalPort WriteEnable { get; protected set; } = new SignalPort();
        public SignalPort OutputEnable { get; protected set; } = new SignalPort();

        public HL256Ram() {
            Initialize(null);
            this.RegisterUpdate();
        }

        public HL256Ram(BitArray[] initialMemory) {
            Initialize(initialMemory);
            this.RegisterUpdate();
        }

        public void Initialize(BitArray[] initialMemory) {
            base.Initialize(AddressSize + DataSize, OutputSize);

            if (initialMemory == null) {
                for (int i = 0; i < MemorySizeInBytes; i++) {
                    memory[i] = new BitArray(DataSize);
                }
            } else {
                for (int i = 0; i < initialMemory.Length; i++) {
                    memory[i] = new BitArray(initialMemory[i]);
                }
                if (initialMemory.Length < MemorySizeInBytes) {
                    for (int i = initialMemory.Length; i < MemorySizeInBytes; i++) {
                        memory[i] = new BitArray(DataSize);
                    }
                }
            }
        }

        public void Update() {
            if (WriteEnable) {
                WriteData();
            } else if (OutputEnable) {
                WriteOutput();
            }
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
            var inputAddress = new BitArray(AddressSize);
            for (int i = 0; i < AddressSize; i++) {
                inputAddress[i] = Inputs[i];
            }

            return inputAddress.ToByteLittleEndian();
        }
    }
}

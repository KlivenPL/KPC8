﻿using Components._Infrastructure.IODevices;
using Components.Signals;
using Infrastructure.BitArrays;
using Simulation.Updates;
using System;
using System.Collections;
using System.Linq;

namespace Components.Rams {
    public class HLRam : IODeviceBase, IRam, IUpdate {
        private readonly int MemorySizeInBytes;
        private readonly int AddressSize;
        private readonly int DataSize;

        private readonly BitArray[] memory;

        public BitArray[] Content => memory;
        public SignalPort WriteEnable { get; protected set; } = new SignalPort();
        public SignalPort OutputEnable { get; protected set; } = new SignalPort();
        public SignalPort Clk { get; protected set; } = new SignalPort();

        public SignalPort[] AddressInputs => Inputs.Take(8).ToArray();
        public SignalPort[] DataInputs => Inputs.Skip(8).ToArray();

        public HLRam(int dataSize, int addressSize, BitArray[] initialMemory = null) {
            MemorySizeInBytes = (int)Math.Pow(2, addressSize);
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
                    memory[i] = new BitArray(initialMemory[i]);
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
            if (OutputEnable) {
                WriteOutput();
            }
        }

        private void Clk_OnEdgeRise() {
            if (WriteEnable) {
                WriteData();
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

        public void Dispose() {
            this.UnregisterUpdate();
        }
    }
}
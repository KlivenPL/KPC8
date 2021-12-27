using Components._Infrastructure.Components;
using Components.Buses;
using Infrastructure.BitArrays;
using KPC8.ControlSignals;
using KPC8.Modules;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tests._Infrastructure;
using Xunit;

namespace Tests.KPC8Tests.ModulesTests {
    public class MemoryModuleTests : TestBase {

        [Fact]
        public void FetchInstructionFromRom() {
            var pc0 = BitArrayHelper.FromString("00000000 00000000");
            var pc1 = BitArrayHelper.FromString("00000000 00000010");

            var rom0 = BitArrayHelper.FromString("11110000");
            var rom1 = BitArrayHelper.FromString("10110000");

            var module = CreateMemoryModule(CreateTestRomData().ToArray(), null, out var dataBus, out var addressBus, out var cs);

            Enable(cs.Pc_oe);
            Enable(cs.Mar_le_hi);
            Enable(cs.Mar_le_lo);

            MakeTickAndWait();

            BitAssert.Equality(pc0, addressBus.Lanes);

            Enable(cs.Pc_ce);
            Enable(cs.Rom_oe);

            MakeTickAndWait();

            BitAssert.Equality(rom0, dataBus.Lanes);

            Enable(cs.Mar_ce);
            Enable(cs.Rom_oe);
            Enable(cs.Pc_ce);
            Enable(cs.Pc_oe);

            MakeTickAndWait();

            BitAssert.Equality(rom1, dataBus.Lanes);
            BitAssert.Equality(pc1, addressBus.Lanes);
        }

        [Fact]
        public void FetchInstructionFromRam() {
            var pc0 = BitArrayHelper.FromString("00000000 00000000");
            var pc1 = BitArrayHelper.FromString("00000000 00000010");

            var ram0 = BitArrayHelper.FromString("10110010");
            var ram1 = BitArrayHelper.FromString("10110001");

            var module = CreateMemoryModule(null, CreateTestRamData().ToArray(), out var dataBus, out var addressBus, out var cs);

            Enable(cs.Pc_oe);
            Enable(cs.Mar_le_hi);
            Enable(cs.Mar_le_lo);

            MakeTickAndWait();

            BitAssert.Equality(pc0, addressBus.Lanes);

            Enable(cs.Pc_ce);
            Enable(cs.Ram_oe);

            MakeTickAndWait();

            BitAssert.Equality(ram0, dataBus.Lanes);

            Enable(cs.Mar_ce);
            Enable(cs.Ram_oe);
            Enable(cs.Pc_ce);
            Enable(cs.Pc_oe);

            MakeTickAndWait();

            BitAssert.Equality(pc1, addressBus.Lanes);
            BitAssert.Equality(ram1, dataBus.Lanes);
        }

        [Fact]
        public void StoreByteInRam() {
            var zero = BitArrayHelper.FromString("00000000");
            var ramAddressHi = BitArrayHelper.FromString("01000100");
            var ramAddressLo = BitArrayHelper.FromString("10000010");
            var ramFullAddress = BitArrayHelper.FromString("01000100 10000010");
            var @byte = BitArrayHelper.FromString("11010111");

            var module = CreateMemoryModule(CreateTestRomData().ToArray(), null, out var dataBus, out var addressBus, out var cs);

            dataBus.Write(ramAddressHi);
            Enable(cs.Mar_le_hi);

            MakeTickAndWait();

            dataBus.Write(ramAddressLo);
            Enable(cs.Mar_le_lo);

            MakeTickAndWait();

            BitAssert.Equality(module.MarContent, ramFullAddress);

            dataBus.Write(@byte);
            Enable(cs.Ram_we);

            MakeTickAndWait();

            dataBus.Write(zero);
            BitAssert.Equality(zero, dataBus.Lanes);

            Enable(cs.Ram_oe);

            MakeTickAndWait();

            BitAssert.Equality(@byte, dataBus.Lanes);
        }

        [Fact]
        public void LoadAddressToMar() {
            var zero = BitArrayHelper.FromString("00000000 00000000");
            var ramAddress = BitArrayHelper.FromString("01000100 10000010");

            var module = CreateMemoryModule(CreateTestRomData().ToArray(), null, out var dataBus, out var addressBus, out var cs);

            addressBus.Write(ramAddress);
            Enable(cs.Mar_le_hi);
            Enable(cs.Mar_le_lo);

            MakeTickAndWait();

            addressBus.Write(zero);
            BitAssert.Equality(zero, addressBus.Lanes);

            Enable(cs.MarToBus_oe);
            MakeTickAndWait();

            BitAssert.Equality(ramAddress, addressBus.Lanes);
        }

        private Memory CreateMemoryModule(BitArray[] romData, BitArray[] ramData, out IBus dataBus, out IBus addressBus, out CsPanel.MemoryPanel csPanel) {
            dataBus = new HLBus("TestDataBus", 8);
            addressBus = new HLBus("TestAddressBus", 16);
            var controlBus = new HLBus("ControlBus", 32);

            var memory = new Memory(romData, ramData, _testClock, dataBus, addressBus);
            csPanel = memory.CreateControlPanel(controlBus);

            return memory;
        }

        private IEnumerable<BitArray> CreateTestRomData() {
            for (int i = 0; i < 65536; i++) {
                if (i == 0) {
                    yield return BitArrayHelper.FromString("11110000");
                } else if (i == 1) {
                    yield return BitArrayHelper.FromString("10110000");
                } else if (i == 65534) {
                    yield return BitArrayHelper.FromString("00001101");
                } else if (i == 65535) {
                    yield return BitArrayHelper.FromString("01001111");
                } else {
                    yield return new BitArray(8);
                }

            }
        }

        private IEnumerable<BitArray> CreateTestRamData() {
            for (int i = 0; i < 65536; i++) {
                if (i == 0) {
                    yield return BitArrayHelper.FromString("10110010");
                } else if (i == 1) {
                    yield return BitArrayHelper.FromString("10110001");
                } else if (i == 65534) {
                    yield return BitArrayHelper.FromString("10000101");
                } else if (i == 65535) {
                    yield return BitArrayHelper.FromString("01001011");
                } else {
                    yield return new BitArray(8);
                }

            }
        }
    }
}

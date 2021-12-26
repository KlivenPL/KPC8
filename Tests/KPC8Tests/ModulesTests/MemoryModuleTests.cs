using Components.Buses;
using Components.Registers;
using Components.Signals;
using Infrastructure.BitArrays;
using KPC8.ControlSignals;
using KPC8.Modules;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tests._Infrastructure;
using Tests.Adapters;
using Xunit;

namespace Tests.KPC8Tests.ModulesTests {
    public class MemoryModuleTests : TestBase {

        [Fact]
        public void FetchInstructionFromRom() {
            var pc0 = BitArrayHelper.FromString("00000000 00000000");
            var pc1 = BitArrayHelper.FromString("00000000 00000010");

            var rom0 = BitArrayHelper.FromString("11110000");
            var rom1 = BitArrayHelper.FromString("10110000");

            //using var irRegisterMock = CreateHiLoRegisterMock(out var inputs, out var outputs, out var outputEnable, out var loadEnable, out var loadEnableHi, out var loadEnableLo, out var clear);
            var module = CreateMemoryModule(CreateTestRom().ToArray(), out var dataBus, out var addressBus, out var cs);

            Enable(cs.Pc_oe);
            Enable(cs.Mar_le);

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


        private Memory CreateMemoryModule(BitArray[] romData, out IBus dataBus, out IBus addressBus, out CsPanel.MemoryPanel csPanel) {
            dataBus = new HLBus("TestDataBus", 8);
            addressBus = new HLBus("TestAddressBus", 16);

            var memory = new Memory(romData, _testClock, dataBus, addressBus);

            csPanel = memory.CreateMemoryPanel();

            return memory;
        }

        private IEnumerable<BitArray> CreateTestRom() {
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

        private HLHiLoRegister CreateHiLoRegisterMock(out Signal[] inputs, out Signal[] outputs, out Signal outputEnable, out Signal loadEnable, out Signal loadEnableHi, out Signal loadEnableLo, out Signal clear) {
            var register = new HLHiLoRegister(16);
            register.Clk.PlugIn(_testClock.Clk);

            inputs = register.CreateSignalAndPlugInInputs().ToArray();
            outputs = register.CreateSignalAndPlugInOutputs().ToArray();

            outputEnable = register.CreateSignalAndPlugInPort(r => r.OutputEnable);
            loadEnableHi = register.CreateSignalAndPlugInPort(r => r.LoadEnableHigh);
            loadEnableLo = register.CreateSignalAndPlugInPort(r => r.LoadEnableLow);
            loadEnable = register.CreateSignalAndPlugInPort(r => r.LoadEnable);
            clear = register.CreateSignalAndPlugInPort(r => r.Clear);

            return register;
        }
    }
}

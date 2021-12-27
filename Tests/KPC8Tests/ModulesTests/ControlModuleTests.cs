using Components._Infrastructure.Components;
using Components.Buses;
using Infrastructure.BitArrays;
using KPC8.ControlSignals;
using KPC8.Modules;
using Tests._Infrastructure;
using Xunit;

namespace Tests.KPC8Tests.ModulesTests {
    public class ControlModuleTests : TestBase {

        [Fact]
        public void LoadInstructionToRegisters() {
            var zero = BitArrayHelper.FromString("00000000");
            var instrHi = BitArrayHelper.FromString("11010001");
            var instrLo = BitArrayHelper.FromString("10100010");
            var totalInstruction = BitArrayHelper.FromString("11010001 10100010");

            var decDestOut = BitArrayHelper.FromString("0100");

            var module = CreateControlModule(out var dataBus, out var registerSelectBus, out var controlBus, out var cs);

            dataBus.Write(instrHi);
            Enable(cs.Ir_le_hi);
            MakeTickAndWait();

            dataBus.Write(instrLo);
            Enable(cs.Ir_le_lo);
            MakeTickAndWait();

            BitAssert.Equality(totalInstruction, module.IrOutput);

            Enable(cs.DecDest_oe);
            MakeTickAndWait();

            BitAssert.Equality(decDestOut, module.DecDestOutput);
        }

        private Control CreateControlModule(out IBus dataBus, out IBus registerSelectBus, out IBus controlBus, out CsPanel.ControlPanel csPanel) {
            dataBus = new HLBus("TestDataBus", 8);
            registerSelectBus = new HLBus("RegisterSelectBus", 16);
            controlBus = new HLBus("ControlBus", 32);

            var control = new Control(null, _testClock, dataBus, registerSelectBus);
            csPanel = control.CreateControlPanel(controlBus);

            return control;
        }
    }
}

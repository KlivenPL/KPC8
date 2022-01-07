using Components._Infrastructure.Components;
using Components.Buses;
using Infrastructure.BitArrays;
using KPC8.ControlSignals;
using KPC8.Modules;
using KPC8.ProgRegs;
using Tests._Infrastructure;
using Xunit;

namespace Tests.KPC8Tests.ModulesTests {
    public class RegistersModuleTests : TestBase {

        [Theory]
        [InlineData(Regs.Zero)]
        [InlineData(Regs.Ass)]
        [InlineData(Regs.Sp)]
        [InlineData(Regs.Fp)]
        [InlineData(Regs.T1)]
        [InlineData(Regs.T2)]
        [InlineData(Regs.T3)]
        [InlineData(Regs.T4)]
        [InlineData(Regs.S1)]
        [InlineData(Regs.S2)]
        [InlineData(Regs.S3)]
        [InlineData(Regs.A1)]
        [InlineData(Regs.A2)]
        [InlineData(Regs.A3)]
        [InlineData(Regs.Rt)]
        [InlineData(Regs.Ra)]
        public void EnableSelectedRegister(Regs selectedRegister) {
            var selectedRegisterAddress = selectedRegister.GetDecodedAddress();

            var module = CreateControlModule(out var dataBus, out var registerSelectBus, out var controlBus, out var cs);

            registerSelectBus.Write(selectedRegisterAddress);
            MakeTickAndWait();

            var selectedIndex = selectedRegister.GetIndex();

            for (int i = 0; i < 16; i++) {
                Assert.Equal(i == selectedIndex, module.IsRegSelected(i));
            }
        }

        [Theory]
        [InlineData(Regs.Ass)]
        [InlineData(Regs.Sp)]
        [InlineData(Regs.Fp)]
        [InlineData(Regs.T1)]
        [InlineData(Regs.T2)]
        [InlineData(Regs.T3)]
        [InlineData(Regs.T4)]
        [InlineData(Regs.S1)]
        [InlineData(Regs.S2)]
        [InlineData(Regs.S3)]
        [InlineData(Regs.A1)]
        [InlineData(Regs.A2)]
        [InlineData(Regs.A3)]
        [InlineData(Regs.Rt)]
        [InlineData(Regs.Ra)]
        public void WriteAndReadSelectedRegister(Regs selectedRegister) {
            var zero = BitArrayHelper.FromString("00000000");
            var data = BitArrayHelper.FromString("10010010");
            var selectedRegisterAddress = selectedRegister.GetDecodedAddress();

            var module = CreateControlModule(out var dataBus, out var registerSelectBus, out var controlBus, out var cs);

            registerSelectBus.Write(selectedRegisterAddress);
            dataBus.Write(data);
            Enable(cs.Regs_L);
            MakeTickAndWait();

            var selectedIndex = selectedRegister.GetIndex();

            BitAssert.Equality(data, module.GetLoRegContent(selectedIndex));

            for (int i = 0; i < 16; i++) {
                if (i != selectedIndex)
                    BitAssert.Equality(zero, module.GetLoRegContent(i));
            }

            dataBus.Write(zero);
            Enable(cs.Regs_O);
            MakeTickAndWait();

            BitAssert.Equality(data, dataBus.Lanes);
        }

        [Theory]
        [InlineData(Regs.Zero)]
        public void WriteAndReadZeroRegister_ValueAlwaysZero(Regs selectedRegister) {
            var zero = BitArrayHelper.FromString("00000000");
            var data = BitArrayHelper.FromString("10010010");
            var selectedRegisterAddress = selectedRegister.GetDecodedAddress();

            var module = CreateControlModule(out var dataBus, out var registerSelectBus, out var controlBus, out var cs);

            registerSelectBus.Write(selectedRegisterAddress);
            dataBus.Write(data);
            Enable(cs.Regs_L);
            MakeTickAndWait();

            var selectedIndex = selectedRegister.GetIndex();

            BitAssert.Equality(zero, module.GetLoRegContent(selectedIndex));

            dataBus.Write(data);
            Enable(cs.Regs_O);
            MakeTickAndWait();

            BitAssert.Equality(zero, dataBus.Lanes);
        }

        private Registers CreateControlModule(out IBus dataBus, out IBus registerSelectBus, out IBus controlBus, out CsPanel.RegsPanel csPanel) {
            dataBus = new HLBus("TestDataBus", 8);
            registerSelectBus = new HLBus("RegisterSelectBus", 16);
            controlBus = new HLBus("ControlBus", 32);

            var control = new Registers(_testClock.Clk, dataBus, registerSelectBus);
            csPanel = control.CreateControlPanel(controlBus);

            return control;
        }
    }
}

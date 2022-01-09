using Components._Infrastructure.Components;
using Components.Buses;
using Infrastructure.BitArrays;
using KPC8.ControlSignals;
using KPC8.CpuFlags;
using KPC8.Modules;
using Tests._Infrastructure;
using Xunit;

namespace Tests.KPC8Tests.ModulesTests {
    public class AluModuleTests : TestBase {

        [Theory]
        [InlineData("11001010")]
        [InlineData("11110001")]
        public void LoadDataToRegA(string dataStr) {
            var data = BitArrayHelper.FromString(dataStr);

            var module = CreateAluModule(out var dataBus, out var flagsBus, out var controlBus, out var cs);

            dataBus.Write(data);
            Enable(cs.RegA_le);
            MakeTickAndWait();

            BitAssert.Equality(data, module.RegAContent);
        }

        [Theory]
        [InlineData("01001110")]
        [InlineData("10110101")]
        public void LoadDataToRegB(string dataStr) {
            var data = BitArrayHelper.FromString(dataStr);

            var module = CreateAluModule(out var dataBus, out var flagsBus, out var controlBus, out var cs);

            dataBus.Write(data);
            Enable(cs.RegB_le);
            MakeTickAndWait();

            BitAssert.Equality(data, module.RegBContent);
        }

        [Theory]
        [InlineData("01001110", "00010101", "01100011", CpuFlag.None)]
        [InlineData("00000000", "11111111", "11111111", CpuFlag.Nf)]
        [InlineData("11111111", "00000000", "11111111", CpuFlag.Nf)]
        [InlineData("00000000", "00000000", "00000000", CpuFlag.Zf)]
        [InlineData("01111111", "00000001", "10000000", CpuFlag.Of | CpuFlag.Nf)]
        [InlineData("11111111", "00000001", "00000000", CpuFlag.Cf | CpuFlag.Zf)]
        [InlineData("11111110", "00001110", "00001100", CpuFlag.Cf)]
        public void AddTwoNumbers(string dataAStr, string dataBStr, string resultStr, CpuFlag flags) {
            var dataA = BitArrayHelper.FromString(dataAStr);
            var dataB = BitArrayHelper.FromString(dataBStr);
            var result = BitArrayHelper.FromString(resultStr);

            var module = CreateAluModule(out var dataBus, out var flagsBus, out var controlBus, out var cs);

            dataBus.Write(dataA);
            Enable(cs.RegA_le);
            MakeTickAndWait();

            dataBus.Write(dataB);
            Enable(cs.RegB_le);
            MakeTickAndWait();

            Enable(cs.Alu_oe);
            MakeTickAndWait();

            BitAssert.Equality(result, dataBus.Lanes);
            BitAssert.Equality(flags.To4BitArray(), flagsBus.Lanes);
        }

        [Theory]
        [InlineData("01001110", "00010101", "00111001", CpuFlag.Cf)]
        [InlineData("00000000", "01111111", "10000001", CpuFlag.Nf)]
        [InlineData("10000000", "01111110", "00000010", CpuFlag.Of | CpuFlag.Cf)]
        [InlineData("00000000", "00000000", "00000000", CpuFlag.Zf | CpuFlag.Cf)]
        [InlineData("01111111", "01111111", "00000000", CpuFlag.Zf | CpuFlag.Cf)]
        [InlineData("11111111", "10000001", "01111110", CpuFlag.Cf)]
        public void SubtractTwoNumbers(string dataAStr, string dataBStr, string resultStr, CpuFlag flags) {
            var dataA = BitArrayHelper.FromString(dataAStr);
            var dataB = BitArrayHelper.FromString(dataBStr);
            var result = BitArrayHelper.FromString(resultStr);

            var module = CreateAluModule(out var dataBus, out var flagsBus, out var controlBus, out var cs);

            dataBus.Write(dataA);
            Enable(cs.RegA_le);
            MakeTickAndWait();

            dataBus.Write(dataB);
            Enable(cs.RegB_le);
            MakeTickAndWait();

            Enable(cs.Alu_c);
            Enable(cs.Alu_oe);
            MakeTickAndWait();

            BitAssert.Equality(result, dataBus.Lanes);
            BitAssert.Equality(flags.To4BitArray(), flagsBus.Lanes);
        }

        [Theory]
        [InlineData("11111110", "00001110", "00001100", CpuFlag.Cf)]
        [InlineData("01111111", "00000001", "10000000", CpuFlag.Of | CpuFlag.Nf)]
        public void AddTwoNumbersAndStoreInRegAAndRegB(string dataAStr, string dataBStr, string resultStr, CpuFlag flags) {
            var dataA = BitArrayHelper.FromString(dataAStr);
            var dataB = BitArrayHelper.FromString(dataBStr);
            var result = BitArrayHelper.FromString(resultStr);

            var module = CreateAluModule(out var dataBus, out var flagsBus, out var controlBus, out var cs);

            dataBus.Write(dataA);
            Enable(cs.RegA_le);
            MakeTickAndWait();

            dataBus.Write(dataB);
            Enable(cs.RegB_le);
            MakeTickAndWait();

            Enable(cs.Alu_oe);
            Enable(cs.RegA_le);
            Enable(cs.RegB_le);
            MakeTickAndWait();

            BitAssert.Equality(result, module.RegAContent);
            BitAssert.Equality(result, module.RegBContent);
            BitAssert.Equality(flags.To4BitArray(), flagsBus.Lanes);
        }

        [Fact]
        public void TransferDataBetweenRegAAndRegB() {
            var zero = BitArrayHelper.FromString("00000000");
            var data = BitArrayHelper.FromString("10101100");

            var module = CreateAluModule(out var dataBus, out var flagsBus, out var controlBus, out var cs);

            dataBus.Write(data);
            Enable(cs.RegA_le);
            MakeTickAndWait();

            BitAssert.Equality(data, module.RegAContent);
            BitAssert.Equality(zero, module.RegBContent);

            dataBus.Write(zero);
            Enable(cs.RegAToBus_oe);
            Enable(cs.RegB_le);
            MakeTickAndWait();

            dataBus.Write(zero);
            Enable(cs.RegA_le);
            MakeTickAndWait();

            BitAssert.Equality(zero, module.RegAContent);
            BitAssert.Equality(data, module.RegBContent);

            Enable(cs.RegBToBus_oe);
            Enable(cs.RegA_le);
            MakeTickAndWait();

            BitAssert.Equality(data, module.RegAContent);
            BitAssert.Equality(data, module.RegBContent);
        }

        private Alu CreateAluModule(out IBus dataBus, out IBus flagsBus, out IBus controlBus, out CsPanel.AluPanel csPanel) {
            dataBus = new HLBus("TestDataBus", 8);
            flagsBus = new HLBus("FlagsBus", 4);
            controlBus = new HLBus("ControlBus", 32);

            var alu = new Alu(_testClock.Clk, dataBus, flagsBus);
            csPanel = alu.CreateControlPanel(controlBus);

            return alu;
        }
    }
}

using Infrastructure.BitArrays;
using KPC8.ProgRegs;
using Tests._Infrastructure;
using Xunit;

namespace Tests.ProgramTests.Multiplication {
    public class MultiplicationProgramTest : ProgramTestBase {

        [Fact]
        public void TestMultiplication() {
            var cp = CompileAndBuildPcModules("MultiplicationProgramSource.kpc", out var modules);
            var zero = BitArrayHelper.FromByteLE(0);
            var result = BitArrayHelper.FromShortLE(777);

            do {
                MakeTickAndWait();
            }
            while (modules.Registers.GetHiRegContent(Regs.Rt.GetIndex()).EqualTo(zero));

            BitAssert.Equality(result, modules.Registers.GetWholeRegContent(Regs.Rt.GetIndex()));
        }
    }
}

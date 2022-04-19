using Infrastructure.BitArrays;
using Tests._Infrastructure;
using Xunit;

namespace Tests.ProgramTests.AsciiCopyToRam {
    public class AsciiCopyToRamTest : ProgramTestBase {

        [Fact]
        public void TestFibonacciSequence() {
            const string TestStr1 = "Just a sample text.";
            const string TestStr2 = "Another other simple text.";

            const ushort RamAddr1 = 0x0;
            const ushort RamAddr2 = 0x30;

            var cp = CompileAndBuildPcModules("AsciiCopyToRamProgramSource.kpc", out var modules);
            var zero = BitArrayHelper.FromShortLE(0);

            TickUntilNop(modules);

            for (int i = 0; i < TestStr1.Length; i++) {
                BitAssert.Equality(BitArrayHelper.FromByteLE((byte)TestStr1[i]), modules.Memory.GetRamAt((ushort)(RamAddr1 + i)));
            }

            for (int i = 0; i < TestStr2.Length; i++) {
                BitAssert.Equality(BitArrayHelper.FromByteLE((byte)TestStr2[i]), modules.Memory.GetRamAt((ushort)(RamAddr2 + i)));
            }
        }
    }
}

using Infrastructure.BitArrays;
using KPC8.ProgRegs;
using Tests._Infrastructure;
using Xunit;

namespace Tests.ProgramTests.Fibonacci {
    public class FibonacciProgramTest : ProgramTestBase {

        [Fact]
        public void TestFibonacciSequence() {
            var cp = CompileAndBuildPcModules("FibonacciProgramSource.kpc", out var modules);
            var zero = BitArrayHelper.FromShortLE(0);

            TickUntilNop(modules);

            BitAssert.Equality(BitArrayHelper.FromByteLE((byte)Fib(12)), modules.Registers.GetLoRegContent(Regs.Rt.GetIndex()));

            for (int i = 0; i <= 12; i++) {
                BitAssert.Equality(BitArrayHelper.FromByteLE((byte)Fib(i)), modules.Memory.GetRamAt((ushort)(255 + i)));
            }
        }

        private static int Fib(int n) {
            if (n <= 1) {
                return n;
            } else {
                return Fib(n - 1) + Fib(n - 2);
            }
        }
    }
}

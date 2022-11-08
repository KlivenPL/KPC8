using Components.Logic;
using Components.Signals;
using Infrastructure.BitArrays;
using System.Linq;
using Tests._Infrastructure;
using Tests.Adapters;
using Xunit;

namespace Tests.ComponentTests {
    public class SingleXorGateTests : TestBase {
        [Fact]
        public void SingleXorFalse() {
            var input = BitArrayHelper.FromString("11");
            var xor = BitArrayHelper.FromString("0");

            using var xorGate = CreateSingleAndGate(out var inputs, out var outputs);

            for (int i = 0; i < 2; i++) {
                inputs[i].Value = input[i];
            }

            MakeTickAndWait();

            BitAssert.Equality(xor, outputs);
        }

        [Fact]
        public void SingleXorFalse_2() {
            var input = BitArrayHelper.FromString("00");
            var xor = BitArrayHelper.FromString("0");

            using var xorGate = CreateSingleAndGate(out var inputs, out var outputs);

            for (int i = 0; i < 2; i++) {
                inputs[i].Value = input[i];
            }

            MakeTickAndWait();

            BitAssert.Equality(xor, outputs);
        }

        [Fact]
        public void SingleXorTrue() {
            var input = BitArrayHelper.FromString("01");
            var and = BitArrayHelper.FromString("1");

            using var xorGate = CreateSingleAndGate(out var inputs, out var outputs);

            for (int i = 0; i < 2; i++) {
                inputs[i].Value = input[i];
            }

            MakeTickAndWait();

            BitAssert.Equality(and, outputs);
        }

        [Fact]
        public void SingleXorTrue_2() {
            var input = BitArrayHelper.FromString("10");
            var and = BitArrayHelper.FromString("1");

            using var xorGate = CreateSingleAndGate(out var inputs, out var outputs);

            for (int i = 0; i < 2; i++) {
                inputs[i].Value = input[i];
            }

            MakeTickAndWait();

            BitAssert.Equality(and, outputs);
        }

        private SingleXorGate CreateSingleAndGate(out Signal[] inputs, out Signal[] outputs) {
            var xorGate = new SingleXorGate("singleXor");

            inputs = xorGate.CreateSignalAndPlugInInputs().ToArray();
            outputs = xorGate.CreateSignalAndPlugInOutputs().ToArray();

            return xorGate;
        }
    }
}

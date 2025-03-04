using Components.Logic;
using Components.Signals;
using Infrastructure.BitArrays;
using System.Linq;
using Tests._Infrastructure;
using Tests.Adapters;
using Xunit;

namespace Tests.ComponentTests {
    public class SingleNandGateTests : TestBase {
        [Fact]
        public void SingleNandFalse() {
            var input = BitArrayHelper.FromString("1101");
            var nand = BitArrayHelper.FromString("1");

            using var nandGate = CreateSingleAndGate(out var inputs, out var outputs);

            for (int i = 0; i < 4; i++) {
                inputs[i].Value = input[i];
            }

            MakeTickAndWait();

            BitAssert.Equality(nand, outputs);
        }

        [Fact]
        public void SingleNandFalse_2() {
            var input = BitArrayHelper.FromString("0000");
            var nand = BitArrayHelper.FromString("1");

            using var nandGate = CreateSingleAndGate(out var inputs, out var outputs);

            for (int i = 0; i < 4; i++) {
                inputs[i].Value = input[i];
            }

            MakeTickAndWait();

            BitAssert.Equality(nand, outputs);
        }

        [Fact]
        public void SingleNandTrue() {
            var input = BitArrayHelper.FromString("1111");
            var and = BitArrayHelper.FromString("0");

            using var nandGate = CreateSingleAndGate(out var inputs, out var outputs);

            for (int i = 0; i < 4; i++) {
                inputs[i].Value = input[i];
            }

            MakeTickAndWait();

            BitAssert.Equality(and, outputs);
        }

        private SingleNandGate CreateSingleAndGate(out Signal[] inputs, out Signal[] outputs) {
            var nandGate = new SingleNandGate("singleNand", 4);

            inputs = nandGate.CreateSignalAndPlugInInputs().ToArray();
            outputs = nandGate.CreateSignalAndPlugInOutputs().ToArray();

            return nandGate;
        }
    }
}

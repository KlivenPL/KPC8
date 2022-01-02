using Components.Logic;
using Components.Signals;
using Infrastructure.BitArrays;
using System.Linq;
using Tests._Infrastructure;
using Tests.Adapters;
using Xunit;

namespace Tests.ComponentTests {
    public class SingleAndGateTests : TestBase {
        [Fact]
        public void SingleAndFalse() {
            var input = BitArrayHelper.FromString("1101");
            var and = BitArrayHelper.FromString("0");

            using var andGate = CreateSingleAndGate(out var inputs, out var outputs);

            for (int i = 0; i < 4; i++) {
                inputs[i].Value = input[i];
            }

            MakeTickAndWait();

            BitAssert.Equality(and, outputs);
        }

        [Fact]
        public void SingleAndTrue() {
            var input = BitArrayHelper.FromString("1111");
            var and = BitArrayHelper.FromString("1");

            using var andGate = CreateSingleAndGate(out var inputs, out var outputs);

            for (int i = 0; i < 4; i++) {
                inputs[i].Value = input[i];
            }

            MakeTickAndWait();

            BitAssert.Equality(and, outputs);
        }

        private SingleAndGate CreateSingleAndGate(out Signal[] inputs, out Signal[] outputs) {
            var andGate = new SingleAndGate("singleAnd", 4);

            inputs = andGate.CreateSignalAndPlugInInputs().ToArray();
            outputs = andGate.CreateSignalAndPlugInOutputs().ToArray();

            return andGate;
        }
    }
}

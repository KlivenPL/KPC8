using Components.Logic;
using Components.Signals;
using Infrastructure.BitArrays;
using System.Linq;
using Tests._Infrastructure;
using Tests.Adapters;
using Xunit;

namespace Tests.ComponentTests {
    public class MultiAndGateTests : TestBase {
        [Fact]
        public void MultiAnd() {
            var inputA = BitArrayHelper.FromString("11001011");
            var inputB = BitArrayHelper.FromString("01110010");

            var and = BitArrayHelper.FromString("01000010");

            using var andGate = CreateMultiAndGate(out var inputs, out var outputs);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = inputA[i];
            }

            for (int i = 8; i < 16; i++) {
                inputs[i].Value = inputB[i - 8];
            }

            MakeTickAndWait();

            BitAssert.Equality(and, outputs);
        }

        private MultiAndGate CreateMultiAndGate(out Signal[] inputs, out Signal[] outputs) {
            var andGate = new MultiAndGate("multiand", 16);

            inputs = andGate.CreateSignalAndPlugInInputs().ToArray();
            outputs = andGate.CreateSignalAndPlugInOutputs().ToArray();

            return andGate;
        }
    }
}

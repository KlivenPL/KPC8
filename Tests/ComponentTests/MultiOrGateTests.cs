using Components.Logic;
using Components.Signals;
using Infrastructure.BitArrays;
using System.Linq;
using Tests._Infrastructure;
using Tests.Adapters;
using Xunit;

namespace Tests.ComponentTests {
    public class MultiOrGateTests : TestBase {
        [Fact]
        public void MultiOr() {
            var inputA = BitArrayHelper.FromString("11011001");
            var inputB = BitArrayHelper.FromString("01110010");

            var or = BitArrayHelper.FromString("11111011");

            using var orGate = CreateMultiOrGate(out var inputs, out var outputs);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = inputA[i];
            }

            for (int i = 8; i < 16; i++) {
                inputs[i].Value = inputB[i - 8];
            }

            MakeTickAndWait();

            BitAssert.Equality(or, outputs);
        }

        private MultiOrGate CreateMultiOrGate(out Signal[] inputs, out Signal[] outputs) {
            var orGate = new MultiOrGate("multior", 16);

            inputs = orGate.CreateSignalAndPlugInInputs().ToArray();
            outputs = orGate.CreateSignalAndPlugInOutputs().ToArray();

            return orGate;
        }
    }
}

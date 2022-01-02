using Components.Logic;
using Components.Signals;
using Infrastructure.BitArrays;
using System.Linq;
using Tests._Infrastructure;
using Tests.Adapters;
using Xunit;

namespace Tests.ComponentTests {
    public class SingleOrGateTests : TestBase {
        [Fact]
        public void SingleOrFalse() {
            var input = BitArrayHelper.FromString("0000");
            var or = BitArrayHelper.FromString("0");

            using var orGate = CreateSingleOrGate(out var inputs, out var outputs);

            for (int i = 0; i < 4; i++) {
                inputs[i].Value = input[i];
            }

            MakeTickAndWait();

            BitAssert.Equality(or, outputs);
        }

        [Fact]
        public void SingleOrTrue() {
            var input = BitArrayHelper.FromString("0100");
            var or = BitArrayHelper.FromString("1");

            using var orGate = CreateSingleOrGate(out var inputs, out var outputs);

            for (int i = 0; i < 4; i++) {
                inputs[i].Value = input[i];
            }

            MakeTickAndWait();

            BitAssert.Equality(or, outputs);
        }

        private SingleOrGate CreateSingleOrGate(out Signal[] inputs, out Signal[] outputs) {
            var andGate = new SingleOrGate("singleOr", 4);

            inputs = andGate.CreateSignalAndPlugInInputs().ToArray();
            outputs = andGate.CreateSignalAndPlugInOutputs().ToArray();

            return andGate;
        }
    }
}

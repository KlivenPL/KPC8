using Components.Logic;
using Components.Signals;
using Infrastructure.BitArrays;
using System.Linq;
using Tests._Infrastructure;
using Tests.Adapters;
using Xunit;

namespace Tests.ComponentTests {
    public class InverterTests : TestBase {
        [Fact]
        public void Invert() {
            var original = BitArrayHelper.FromString("10001001");
            var inverted = BitArrayHelper.FromString("01110110");

            using var inverter = CreateInverter(out var inputs, out var outputs);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = original[i];
            }

            MakeTickAndWait();

            BitAssert.Equality(inverted, outputs);
        }

        private Inverter CreateInverter(out Signal[] inputs, out Signal[] outputs) {
            var inverter = new Inverter(8);

            inputs = inverter.CreateSignalAndPlugInInputs().ToArray();
            outputs = inverter.CreateSignalAndPlugInOutputs().ToArray();

            return inverter;
        }
    }
}

using Components.Signals;
using Components.Transcievers;
using Infrastructure.BitArrays;
using System.Linq;
using Tests._Infrastructure;
using Tests.Adapters;
using Xunit;

namespace Tests.ComponentTests {
    public class HLTranscieverTests : TestBase {

        [Fact]
        public void TranscieveOrBlockInput() {
            var inputData = BitArrayHelper.FromString("11110001");
            var zero = BitArrayHelper.FromString("00000000");

            using var transciever = CreateTransciever(out var inputs, out var outputs, out var outputEnable);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = inputData[i];
            }

            outputEnable.Value = true;
            MakeTickAndWait();

            BitAssert.Equality(inputData, outputs);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = zero[i];
            }

            outputEnable.Value = false;
            MakeTickAndWait();

            BitAssert.Equality(inputData, outputs);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = zero[i];
            }

            outputEnable.Value = true;
            MakeTickAndWait();

            BitAssert.Equality(zero, outputs);
        }

        [Fact]
        public void DoNothing() {
            var inputData = BitArrayHelper.FromString("11110001");
            var zero = BitArrayHelper.FromString("00000000");

            using var transciever = CreateTransciever(out var inputs, out var outputs, out var outputEnable);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = inputData[i];
            }

            MakeTickAndWait();

            BitAssert.Equality(zero, outputs);

            MakeTickAndWait();

            BitAssert.Equality(zero, outputs);
        }

        private HLTransciever CreateTransciever(out Signal[] inputs, out Signal[] outputs, out Signal outputEnable) {
            var register = new HLTransciever(8);
            inputs = register.CreateSignalAndPlugInInputs().ToArray();
            outputs = register.CreateSignalAndPlugInOutputs().ToArray();

            outputEnable = register.CreateSignalAndPlugInPort(r => r.OutputEnable);

            return register;
        }
    }
}

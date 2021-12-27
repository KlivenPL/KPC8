using Components.Decoders;
using Components.Signals;
using Infrastructure.BitArrays;
using System.Linq;
using Tests._Infrastructure;
using Tests.Adapters;
using Xunit;

namespace Tests.ComponentTests {
    public class HLDecoderTests : TestBase {

        [Theory]
        [InlineData("0", "10")]
        [InlineData("00", "1000")]
        [InlineData("01", "0100")]
        [InlineData("10", "0010")]
        [InlineData("11", "0001")]
        [InlineData("1010", "00000000 00100000")]
        public void Decode(string input, string output) {
            var encoded = BitArrayHelper.FromString(input);
            var decoded = BitArrayHelper.FromString(output);

            using var inverter = CreateDecoder(encoded.Length, out var inputs, out var outputs, out var outputEnable);

            for (int i = 0; i < encoded.Length; i++) {
                inputs[i].Value = encoded[i];
            }

            outputEnable.Value = true;
            MakeTickAndWait();

            BitAssert.Equality(decoded, outputs);
        }

        private HLDecoder CreateDecoder(int inputSize, out Signal[] inputs, out Signal[] outputs, out Signal outputEnable) {
            var decoder = new HLDecoder(inputSize);

            inputs = decoder.CreateSignalAndPlugInInputs().ToArray();
            outputs = decoder.CreateSignalAndPlugInOutputs().ToArray();

            outputEnable = decoder.CreateSignalAndPlugInPort(x => x.OutputEnable);
            return decoder;
        }
    }
}

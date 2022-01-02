using Components.Multiplexers;
using Components.Signals;
using Infrastructure.BitArrays;
using System.Linq;
using Tests._Infrastructure;
using Tests.Adapters;
using Xunit;

namespace Tests.ComponentTests {
    public class HLSingleSwitch2NToNMuxTests : TestBase {

        [Theory]
        [InlineData("1010", "0111")]
        [InlineData("11110000", "00001111")]
        [InlineData("101001", "010110")]
        public void SelectInput(string inputAStr, string inputBStr) {
            var inputA = BitArrayHelper.FromString(inputAStr);
            var inputB = BitArrayHelper.FromString(inputBStr);

            using var hl2NToNMux = CreateMux(inputA.Length, out var inputs, out var outputs, out var selectB);

            for (int i = 0; i < inputA.Length; i++) {
                inputs[i].Value = inputA[i];
                inputs[i + inputA.Length].Value = inputB[i];
            }

            Enable(selectB);
            MakeTickAndWait();

            BitAssert.Equality(inputB, outputs);

            MakeTickAndWait();

            BitAssert.Equality(inputA, outputs);
        }

        private HLSingleSwitch2NToNMux CreateMux(int singleInputSize, out Signal[] inputs, out Signal[] outputs, out Signal selectB) {
            var hLSingleSwitch2NToNMux = new HLSingleSwitch2NToNMux("hLSingleSwitch2NToNMux", singleInputSize);

            inputs = hLSingleSwitch2NToNMux.CreateSignalAndPlugInInputs().ToArray();
            outputs = hLSingleSwitch2NToNMux.CreateSignalAndPlugInOutputs().ToArray();

            selectB = hLSingleSwitch2NToNMux.CreateSignalAndPlugInPort(x => x.SelectB);
            return hLSingleSwitch2NToNMux;
        }
    }
}

using _Infrastructure.BitArrays;
using Components.Signals;
using Components.Transcievers;
using Infrastructure.BitArrays;
using System.Linq;
using Tests._Infrastructure;
using Tests.Adapters;
using Xunit;

namespace Tests.ComponentTests {
    public class HLHiLoTranscieverTests : TestBase {

        [Fact]
        public void TranscieveOrBlockInput() {
            var inputData = BitArrayHelper.FromString("11110001");
            var zero = BitArrayHelper.FromString("00000000");
            var halfZero = BitArrayHelper.FromString("0000");

            using var transciever = CreateTransciever(out var inputs, out var outputs, out var outputEnableHi, out var outputEnableLo);

            inputs.Write(inputData);

            Enable(outputEnableHi);
            MakeTickAndWait();

            BitAssert.Equality(inputData.Take(4).MergeWith(halfZero), outputs);

            outputs.Write(zero);

            Enable(outputEnableLo);
            MakeTickAndWait();

            BitAssert.Equality(halfZero.MergeWith(inputData.Skip(4).Take(4)), outputs);

            outputs.Write(zero);
            Enable(outputEnableLo);
            Enable(outputEnableHi);
            MakeTickAndWait();

            BitAssert.Equality(inputData, outputs);

            outputs.Write(zero);
            MakeTickAndWait();

            BitAssert.Equality(zero, outputs);
        }

        [Fact]
        public void DoNothing() {
            var inputData = BitArrayHelper.FromString("11110001");
            var zero = BitArrayHelper.FromString("00000000");

            using var transciever = CreateTransciever(out var inputs, out var outputs, out var outputEnableHi, out var outputEnableLo);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = inputData[i];
            }

            MakeTickAndWait();

            BitAssert.Equality(zero, outputs);

            MakeTickAndWait();

            BitAssert.Equality(zero, outputs);
        }

        private HLHiLoTransciever CreateTransciever(out Signal[] inputs, out Signal[] outputs, out Signal outputEnableHi, out Signal outputEnableLo) {
            var register = new HLHiLoTransciever("transciever", 8);
            inputs = register.CreateSignalAndPlugInInputs().ToArray();
            outputs = register.CreateSignalAndPlugInOutputs().ToArray();

            outputEnableHi = register.CreateSignalAndPlugInPort(r => r.OutputEnableHi);
            outputEnableLo = register.CreateSignalAndPlugInPort(r => r.OutputEnableLo);

            return register;
        }
    }
}

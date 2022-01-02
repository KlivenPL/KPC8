using Components.Registers;
using Components.Signals;
using Infrastructure.BitArrays;
using System.Linq;
using Tests._Infrastructure;
using Tests.Adapters;
using Xunit;

namespace Tests.ComponentTests {
    public class HLHiLoRegisterTests : TestBase {

        [Fact]
        public void LoadAndOutputData() {
            var zero = BitArrayHelper.FromString("00000000 00000000");
            var data1 = BitArrayHelper.FromString("00000000 11110001");
            var data2 = BitArrayHelper.FromString("11000100 11000100");

            var mixedData = BitArrayHelper.FromString("11000100 11110001");

            using var register = CreateHiLoRegister(out var inputs, out var outputs, out var outputEnable, out var loadEnable, out var loadEnableHi, out var loadEnableLo, out var clear);

            for (int i = 0; i < 16; i++) {
                inputs[i].Value = data1[i];
            }

            loadEnable.Value = true;

            MakeTickAndWait();

            BitAssert.Equality(data1, register.Content);
            BitAssert.Equality(zero, outputs);

            loadEnable.Value = false;
            outputEnable.Value = true;

            MakeTickAndWait();

            BitAssert.Equality(data1, outputs);

            for (int i = 0; i < 16; i++) {
                inputs[i].Value = data2[i];
            }

            loadEnableHi.Value = true;
            loadEnable.Value = true;

            MakeTickAndWait();

            BitAssert.Equality(mixedData, outputs);

            for (int i = 0; i < 16; i++) {
                inputs[i].Value = data2[i];
            }

            loadEnableHi.Value = false;
            loadEnableLo.Value = true;

            MakeTickAndWait();

            BitAssert.Equality(data2, outputs);

        }

        private HLHiLoRegister CreateHiLoRegister(out Signal[] inputs, out Signal[] outputs, out Signal outputEnable, out Signal loadEnable, out Signal loadEnableHi, out Signal loadEnableLo, out Signal clear) {
            var register = new HLHiLoRegister("hiloregister", 16);
            register.Clk.PlugIn(_testClock.Clk);

            inputs = register.CreateSignalAndPlugInInputs().ToArray();
            outputs = register.CreateSignalAndPlugInOutputs().ToArray();

            outputEnable = register.CreateSignalAndPlugInPort(r => r.OutputEnable);
            loadEnableHi = register.CreateSignalAndPlugInPort(r => r.LoadEnableHigh);
            loadEnableLo = register.CreateSignalAndPlugInPort(r => r.LoadEnableLow);
            loadEnable = register.CreateSignalAndPlugInPort(r => r.LoadEnable);
            clear = register.CreateSignalAndPlugInPort(r => r.Clear);

            return register;
        }
    }
}

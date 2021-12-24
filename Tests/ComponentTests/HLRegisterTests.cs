using Components.Registers;
using Components.Signals;
using Infrastructure.BitArrays;
using System.Linq;
using Tests.Adapters;
using Xunit;

namespace Tests.ComponentTests {
    public class HLRegisterTests : TestBase {

        [Fact]
        public void LoadAndOutputData() {
            var data = BitArrayHelper.FromString("11110001");

            using var register = CreateRegister(out var inputs, out var outputs, out var enableSig, out var loadSig, out _);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = data[i];
            }

            loadSig.Value = true;

            MakeTickAndWait();

            Assert.True(register.Content.EqualTo(data));

            for (int i = 0; i < 8; i++) {
                Assert.False(outputs[i]);
            }

            loadSig.Value = false;
            enableSig.Value = true;

            MakeTickAndWait();

            for (int i = 0; i < 8; i++) {
                Assert.Equal(outputs[i], data[i]);
            }
        }

        [Fact]
        public void DoNothing() {
            var data = BitArrayHelper.FromString("11110001");

            using var register = CreateRegister(out var inputs, out var outputs, out _, out _, out _);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = data[i];
            }

            MakeTickAndWait();

            Assert.False(register.Content.EqualTo(data));

            for (int i = 0; i < 8; i++) {
                Assert.False(outputs[i]);
            }


            MakeTickAndWait();

            for (int i = 0; i < 8; i++) {
                Assert.False(outputs[i]);
            }

            Assert.False(register.Content.EqualTo(data));
        }

        [Fact]
        public void Clear() {
            var data = BitArrayHelper.FromString("11110001");
            var zeroData = BitArrayHelper.FromString("00000000");

            using var register = CreateRegister(out var inputs, out _, out _, out var loadSig, out var clearSig);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = data[i];
            }

            loadSig.Value = true;

            MakeTickAndWait();


            Assert.True(register.Content.EqualTo(data));

            loadSig.Value = false;
            clearSig.Value = true;

            MakeTickAndWait();

            Assert.True(register.Content.EqualTo(zeroData));
        }

        private HLRegister CreateRegister(out Signal[] inputs, out Signal[] outputs, out Signal enableSig, out Signal loadSig, out Signal clearSig) {
            var register = new HLRegister(8);
            register.Clk.PlugIn(_testClock.Clk);

            inputs = register.CreateSignalAndPlugInInputs().ToArray();
            outputs = register.CreateSignalAndPlugInOutputs().ToArray();

            enableSig = register.CreateSignalAndPlugInPort(r => r.OutputEnable);
            loadSig = register.CreateSignalAndPlugInPort(r => r.LoadEnable);
            clearSig = register.CreateSignalAndPlugInPort(r => r.Clear);

            return register;
        }
    }
}

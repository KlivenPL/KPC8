using Components.Counters;
using Components.Signals;
using Infrastructure.BitArrays;
using System.Linq;
using Tests._Infrastructure;
using Tests.Adapters;
using Xunit;

namespace Tests.ComponentTests {
    public class HLHiLoCounterTests : TestBase {

        [Fact]
        public void LoadDataAndWait() {
            var data1 = BitArrayHelper.FromString("01010101 01010101");
            var data1Zeroed = BitArrayHelper.FromString("01010101 00000000");

            var data2 = BitArrayHelper.FromString("10001000 10001000");
            var dataMixed = BitArrayHelper.FromString("01010101 10001000");

            using var counter = CreateHiLoCounter(out var inputs, out var outputs, out var outputEnable, out var loadEnable, out var loadEnableHi, out var loadEnableLo, out var countEnable, out var clear);

            for (int i = 0; i < 16; i++) {
                inputs[i].Value = data1[i];
            }

            loadEnable.Value = true;

            loadEnableHi.Value = true;
            MakeTickAndWait();

            BitAssert.Equality(data1Zeroed, counter.Content);

            for (int i = 0; i < 16; i++) {
                inputs[i].Value = data2[i];
            }

            loadEnableHi.Value = false;
            loadEnableLo.Value = true;
            MakeTickAndWait();

            BitAssert.Equality(dataMixed, counter.Content);

            loadEnable.Value = false;
            outputEnable.Value = true;
            MakeTickAndWait();

            BitAssert.Equality(dataMixed, outputs);
        }

        private HLHiLoCounter CreateHiLoCounter(out Signal[] inputs, out Signal[] outputs, out Signal outputEnable, out Signal loadEnable, out Signal loadEnableHi, out Signal loadEnableLo, out Signal countEnable, out Signal clear) {
            var counter = new HLHiLoCounter(16);
            counter.Clk.PlugIn(_testClock.Clk);

            inputs = counter.CreateSignalAndPlugInInputs().ToArray();
            outputs = counter.CreateSignalAndPlugInOutputs().ToArray();

            outputEnable = counter.CreateSignalAndPlugInPort(r => r.OutputEnable);
            loadEnable = counter.CreateSignalAndPlugInPort(r => r.LoadEnable);
            loadEnableHi = counter.CreateSignalAndPlugInPort(r => r.LoadEnableHigh);
            loadEnableLo = counter.CreateSignalAndPlugInPort(r => r.LoadEnableLow);
            countEnable = counter.CreateSignalAndPlugInPort(r => r.CountEnable);
            clear = counter.CreateSignalAndPlugInPort(r => r.Clear);

            return counter;
        }
    }
}

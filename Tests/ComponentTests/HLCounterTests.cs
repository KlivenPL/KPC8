using _Infrastructure.BitArrays;
using Components.Counters;
using Components.Signals;
using Infrastructure.BitArrays;
using System.Linq;
using Tests._Infrastructure;
using Tests.Adapters;
using Xunit;

namespace Tests.ComponentTests {
    public class HLCounterTests : TestBase {

        [Fact]
        public void TwoClockCyclesCount() {
            var firstCount = BitArrayHelper.FromString("00000001");
            var secondCount = BitArrayHelper.FromString("00000010");

            using var counter = CreateCounter(out var inputs, out var outputs, out var outputEnable, out var loadEnable, out var countEnable, out var clear);

            countEnable.Value = true;
            MakeTickAndWait();

            BitAssert.Equality(firstCount, counter.Content);

            outputEnable.Value = true;
            MakeTickAndWait();

            BitAssert.Equality(secondCount, outputs);
        }

        [Fact]
        public void OverflowCountResetToZero() {
            var data = BitArrayHelper.FromString("11111111");
            var zero = BitArrayHelper.FromString("00000000");
            var one = BitArrayHelper.FromString("00000001");

            using var counter = CreateCounter(out var inputs, out var outputs, out var outputEnable, out var loadEnable, out var countEnable, out var clear);

            inputs.Write(data);

            loadEnable.Value = true;
            MakeTickAndWait();

            loadEnable.Value = false;
            countEnable.Value = true;
            MakeTickAndWait();

            BitAssert.Equality(zero, counter.Content);

            outputEnable.Value = true;
            MakeTickAndWait();

            BitAssert.Equality(one, outputs);
        }

        [Fact]
        public void LoadDataAndWait() {
            var data = BitArrayHelper.FromString("01010101");

            using var counter = CreateCounter(out var inputs, out var outputs, out var outputEnable, out var loadEnable, out var countEnable, out var clear);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = data[i];
            }

            loadEnable.Value = true;
            MakeTickAndWait();

            BitAssert.Equality(data, counter.Content);

            loadEnable.Value = false;
            MakeTickAndWait();

            BitAssert.Equality(data, counter.Content);

            outputEnable.Value = true;
            MakeTickAndWait();

            BitAssert.Equality(data, outputs);
        }

        private HLCounter CreateCounter(out Signal[] inputs, out Signal[] outputs, out Signal outputEnable, out Signal loadEnable, out Signal countEnable, out Signal clear) {
            var counter = new HLCounter("hlCounter",8);
            counter.Clk.PlugIn(_testClock.Clk);

            inputs = counter.CreateSignalAndPlugInInputs().ToArray();
            outputs = counter.CreateSignalAndPlugInOutputs().ToArray();

            outputEnable = counter.CreateSignalAndPlugInPort(r => r.OutputEnable);
            loadEnable = counter.CreateSignalAndPlugInPort(r => r.LoadEnable);
            countEnable = counter.CreateSignalAndPlugInPort(r => r.CountEnable);
            clear = counter.CreateSignalAndPlugInPort(r => r.Clear);

            return counter;
        }
    }
}

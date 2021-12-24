using _Infrastructure.BitArrays;
using Components.Rams;
using Components.Signals;
using Infrastructure.BitArrays;
using System.Collections;
using System.Linq;
using Tests.Adapters;
using Xunit;

namespace Tests.ComponentTests {
    public class HL256RamTests : TestBase {

        [Fact]
        public void WriteAndOutputData() {
            var zero = BitArrayHelper.FromString("00000000");
            var address = BitArrayHelper.FromString("01001111");
            var data = BitArrayHelper.FromString("11001111");

            var ram = CreateRam(out var inputs, out var outputs, out var outputEnable, out var writeEnable);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = address[i];
                inputs[i + 8].Value = data[i];
            }

            writeEnable.Value = true;
            MakeTickAndWait();

            Assert.True(ram.Content[address.ToByteLittleEndian()].EqualTo(data));
            Assert.True(outputs.ToBitArray().EqualTo(zero));

            writeEnable.Value = false;
            outputEnable.Value = true;
            MakeTickAndWait();

            Assert.True(ram.Content[address.ToByteLittleEndian()].EqualTo(data));
            Assert.True(outputs.ToBitArray().EqualTo(data));
        }

        [Fact]
        public void InitializeWithData() {
            var zero = BitArrayHelper.FromString("00000000");
            var data = BitArrayHelper.FromString("11001111");
            var address = BitArrayHelper.FromString("00000011");
            var lastAddress = BitArrayHelper.FromString("11111111");

            var initialMemory = new BitArray[] {
                new BitArray(8),
                new BitArray(8),
                new BitArray(8),
                data
            };

            var ram = CreateRam(out var inputs, out var outputs, out var outputEnable, out var writeEnable, initialMemory);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = address[i];
            }

            outputEnable.Value = true;
            MakeTickAndWait();

            Assert.True(ram.Content[address.ToByteLittleEndian()].EqualTo(data));
            Assert.True(ram.Content[lastAddress.ToByteLittleEndian()].EqualTo(zero));
            Assert.True(outputs.ToBitArray().EqualTo(data));
        }

        private HLRam CreateRam(out Signal[] inputs, out Signal[] outputs, out Signal outputEnable, out Signal writeEnable, BitArray[] initialMemory = null) {
            HLRam ram = null;

            if (initialMemory == null) {
                ram = new HLRam(8, 8);
            } else {
                ram = new HLRam(8, 8, initialMemory);
            }

            ram.Clk.PlugIn(_testClock.Clk);
            inputs = ram.CreateSignalAndPlugInInputs().ToArray();
            outputs = ram.CreateSignalAndPlugInOutputs().ToArray();

            outputEnable = ram.CreateSignalAndPlugInPort(r => r.OutputEnable);
            writeEnable = ram.CreateSignalAndPlugInPort(r => r.WriteEnable);

            return ram;
        }
    }
}

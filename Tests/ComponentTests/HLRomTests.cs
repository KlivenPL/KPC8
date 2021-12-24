using Components.Roms;
using Components.Signals;
using Infrastructure.BitArrays;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tests._Infrastructure;
using Tests.Adapters;
using Xunit;

namespace Tests.ComponentTests {
    public class HLRomTests : TestBase {

        [Fact]
        public void InitializeWithoutData() {
            var zero = BitArrayHelper.FromString("00000000");
            var address0 = BitArrayHelper.FromString("00000000 00000000");
            var address65535 = BitArrayHelper.FromString("11111111 11111111");

            using var rom = CreateRom(null, out var inputs, out var outputs, out var outputEnable);

            for (int i = 0; i < 16; i++) {
                inputs[i].Value = address0[i];
            }

            outputEnable.Value = true;
            MakeTickAndWait();

            BitAssert.Equality(zero, outputs);

            for (int i = 0; i < 16; i++) {
                inputs[i].Value = address65535[i];
            }

            MakeTickAndWait();
            BitAssert.Equality(zero, outputs);
        }

        [Fact]
        public void InitializeWithData() {
            var address0 = BitArrayHelper.FromString("00000000 00000000");
            var address0Data = BitArrayHelper.FromString("11110000");

            var address1 = BitArrayHelper.FromString("00000000 00000001");
            var address1Data = BitArrayHelper.FromString("10110000");

            var address65534 = BitArrayHelper.FromString("11111111 11111110");
            var address65534Data = BitArrayHelper.FromString("00001101");

            var address65535 = BitArrayHelper.FromString("11111111 11111111");
            var address65535Data = BitArrayHelper.FromString("00001111");

            using var rom = CreateRom(CreateTestMemory().ToArray(), out var inputs, out var outputs, out var outputEnable);

            for (int i = 0; i < 16; i++) {
                inputs[i].Value = address0[i];
            }

            outputEnable.Value = true;
            MakeTickAndWait();

            BitAssert.Equality(address0Data, outputs);

            for (int i = 0; i < 16; i++) {
                inputs[i].Value = address1[i];
            }

            MakeTickAndWait();

            BitAssert.Equality(address1Data, outputs);

            for (int i = 0; i < 16; i++) {
                inputs[i].Value = address65534[i];
            }

            MakeTickAndWait();

            BitAssert.Equality(address65534Data, outputs);

            for (int i = 0; i < 16; i++) {
                inputs[i].Value = address65535[i];
            }

            MakeTickAndWait();
            BitAssert.Equality(address65535Data, outputs);
        }

        private HLRom CreateRom(BitArray[] initialMemory, out Signal[] inputs, out Signal[] outputs, out Signal outputEnable) {
            HLRom rom = new HLRom(8, 16, initialMemory);

            inputs = rom.CreateSignalAndPlugInInputs().ToArray();
            outputs = rom.CreateSignalAndPlugInOutputs().ToArray();

            outputEnable = rom.CreateSignalAndPlugInPort(r => r.OutputEnable);
            return rom;
        }

        private IEnumerable<BitArray> CreateTestMemory() {
            for (int i = 0; i < 65536; i++) {
                if (i == 0) {
                    yield return BitArrayHelper.FromString("11110000");
                } else if (i == 1) {
                    yield return BitArrayHelper.FromString("10110000");
                } else if (i == 65534) {
                    yield return BitArrayHelper.FromString("00001101");
                } else if (i == 65535) {
                    yield return BitArrayHelper.FromString("00001111");
                } else {
                    yield return new BitArray(8);
                }

            }
        }
    }
}

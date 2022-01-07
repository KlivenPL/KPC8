using _Infrastructure.BitArrays;
using Components.Registers;
using Components.Signals;
using Infrastructure.BitArrays;
using System.Linq;
using Tests._Infrastructure;
using Tests.Adapters;
using Xunit;

namespace Tests.ComponentTests {
    public class HLLohRegisterTests : TestBase {

        [Fact]
        public void LoadAndOutputData() {
            var zero8 = BitArrayHelper.FromString("00000000");
            var zero16 = BitArrayHelper.FromString("00000000 00000000");
            var data = BitArrayHelper.FromString("11000010 11110001");

            var dataB = BitArrayHelper.FromString("01010011 01100110");

            using var register = CreateHiLoRegister(out var inputs, out var outputs, out var L, out var O, out var H, out var chipEnable);

            inputs.Write(data);

            // nothing
            Enable(chipEnable);
            MakeTickAndWait();
            BitAssert.Equality(zero16, register.Content);
            BitAssert.Equality(zero16, outputs);

            // nothing with no CE
            Enable(L);
            MakeTickAndWait();
            BitAssert.Equality(zero16, register.Content);
            BitAssert.Equality(zero16, outputs);

            // load lower
            Enable(chipEnable);
            Enable(L);

            MakeTickAndWait();

            BitAssert.Equality(data.Slice(8, 8), register.Content.Slice(8, 8));
            BitAssert.Equality(zero8, register.Content.Take(8));
            BitAssert.Equality(zero16, outputs);

            // load higher
            Enable(chipEnable);
            Enable(L);
            Enable(H);

            MakeTickAndWait();

            BitAssert.Equality(data, register.Content);
            BitAssert.Equality(zero16, outputs);

            // out lower
            Enable(chipEnable);
            Enable(O);

            MakeTickAndWait();

            BitAssert.Equality(data, register.Content);
            BitAssert.Equality(data.Slice(8, 8), outputs.TakeLast(8).ToList());

            // clear
            Enable(chipEnable);
            Enable(L);
            Enable(O);
            Enable(H);

            MakeTickAndWait();

            BitAssert.Equality(zero16, register.Content);

            inputs.Write(dataB);

            // load whole
            Enable(chipEnable);
            Enable(L);
            Enable(O);

            MakeTickAndWait();

            BitAssert.Equality(dataB, register.Content);

            // out whole
            Enable(chipEnable);
            Enable(H);

            MakeTickAndWait();

            BitAssert.Equality(dataB, register.Content);
            BitAssert.Equality(dataB, outputs);
        }

        private HLLohRegister CreateHiLoRegister(out Signal[] inputs, out Signal[] outputs, out Signal L, out Signal O, out Signal H, out Signal chipEnable) {
            var register = new HLLohRegister("HLLohRegister", 16);
            register.Clk.PlugIn(_testClock.Clk);

            inputs = register.CreateSignalAndPlugInInputs().ToArray();
            outputs = register.CreateSignalAndPlugInOutputs().ToArray();

            L = register.CreateSignalAndPlugInPort(r => r.L);
            O = register.CreateSignalAndPlugInPort(r => r.O);
            H = register.CreateSignalAndPlugInPort(r => r.H);
            chipEnable = register.CreateSignalAndPlugInPort(r => r.ChipEnable);

            return register;
        }
    }
}

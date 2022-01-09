using _Infrastructure.BitArrays;
using Components.Adders;
using Components.Signals;
using Infrastructure.BitArrays;
using System.Linq;
using Tests._Infrastructure;
using Tests.Adapters;
using Xunit;

namespace Tests.ComponentTests {
    public class HLAdderTests : TestBase {

        [Fact]
        public void LoadAddAndOutput() {
            var dataA = BitArrayHelper.FromString("10101100");
            var dataB = BitArrayHelper.FromString("11001111");

            var addResult = BitArrayHelper.FromString("01111100");

            using var adder = CreateAdder(out var inputs, out var outputs, out var outputEnable, out var a, out var b, out var c, out var carryIn, out var carryOut, out var negativeFlag, out var zeroFlag, out var overflowFlag);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = dataA[i];
                inputs[i + 8].Value = dataB[i];
            }

            carryIn.Value = true;
            outputEnable.Value = true;

            MakeTickAndWait();

            Assert.True(adder.Content.EqualTo(addResult));
            Assert.True(outputs.ToBitArray().EqualTo(addResult));
            Assert.True(carryOut);
            Assert.True(overflowFlag);
            Assert.False(zeroFlag);
            Assert.False(negativeFlag);
        }

        [Fact]
        public void Add16BitNumbers() {
            var dataALo = BitArrayHelper.FromString("11110000");
            var dataAHi = BitArrayHelper.FromString("11011000");
            var dataBLo = BitArrayHelper.FromString("11010000");
            var dataBHi = BitArrayHelper.FromString("10001010");

            var addResult1 = BitArrayHelper.FromString("11000000");
            var addResult2 = BitArrayHelper.FromString("01100011");
            //var addResult1 = BitArrayHelper.FromString("11000000");

            using var adder = CreateAdder(out var inputs, out var outputs, out var outputEnable, out var a, out var b, out var c, out var carryIn, out var carryOut, out var negativeFlag, out var zeroFlag, out var overflowFlag);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = dataALo[i];
                inputs[i + 8].Value = dataBLo[i];
            }

            outputEnable.Value = true;

            MakeTickAndWait();

            BitAssert.Equality(addResult1, adder.Content);
            BitAssert.Equality(addResult1, outputs);

            Assert.True(carryOut);
            Assert.False(overflowFlag);
            Assert.False(zeroFlag);
            Assert.True(negativeFlag);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = dataAHi[i];
                inputs[i + 8].Value = dataBHi[i];
            }

            carryIn.Value = carryOut.Value;
            outputEnable.Value = true;

            MakeTickAndWait();

            BitAssert.Equality(addResult2, adder.Content);
            BitAssert.Equality(addResult2, outputs);

            Assert.True(carryOut);
            Assert.True(overflowFlag);
            Assert.False(zeroFlag);
            Assert.False(negativeFlag);
        }

        [Fact]
        public void AddNoCarryOut() {
            var dataA = BitArrayHelper.FromString("10101100");
            var dataB = BitArrayHelper.FromString("00001111");

            var addResult = BitArrayHelper.FromString("10111011");

            using var adder = CreateAdder(out var inputs, out var outputs, out var outputEnable, out var a, out var b, out var c, out var carryIn, out var carryOut, out var negativeFlag, out var zeroFlag, out var overflowFlag);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = dataA[i];
                inputs[i + 8].Value = dataB[i];
            }

            outputEnable.Value = true;

            MakeTickAndWait();

            Assert.True(adder.Content.EqualTo(addResult));
            Assert.True(outputs.ToBitArray().EqualTo(addResult));
            Assert.False(carryOut);
            Assert.False(overflowFlag);
            Assert.False(zeroFlag);
            Assert.True(negativeFlag);
        }


        [Fact]
        public void Substract() {
            var dataA = BitArrayHelper.FromString("11001111");
            var dataB = BitArrayHelper.FromString("10101100");

            var addResult = BitArrayHelper.FromString("00100011");

            using var adder = CreateAdder(out var inputs, out var outputs, out var outputEnable, out var a, out var b, out var c, out var carryIn, out var carryOut, out var negativeFlag, out var zeroFlag, out var overflowFlag);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = dataA[i];
                inputs[i + 8].Value = dataB[i];
            }

            outputEnable.Value = true;
            c.Value = true;

            MakeTickAndWait();

            Assert.True(adder.Content.EqualTo(addResult));
            Assert.True(outputs.ToBitArray().EqualTo(addResult));
            Assert.True(carryOut);
            Assert.False(overflowFlag);
            Assert.False(zeroFlag);
            Assert.False(negativeFlag);
        }

        [Fact]
        public void Substract_ResultIsZero() {
            var dataA = BitArrayHelper.FromString("01001111");
            var dataB = BitArrayHelper.FromString("01001111");

            var addResult = BitArrayHelper.FromString("00000000");

            using var adder = CreateAdder(out var inputs, out var outputs, out var outputEnable, out var a, out var b, out var c, out var carryIn, out var carryOut, out var negativeFlag, out var zeroFlag, out var overflowFlag);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = dataA[i];
                inputs[i + 8].Value = dataB[i];
            }

            outputEnable.Value = true;
            c.Value = true;

            MakeTickAndWait();

            Assert.True(adder.Content.EqualTo(addResult));
            Assert.True(outputs.ToBitArray().EqualTo(addResult));
            Assert.True(carryOut);
            Assert.False(overflowFlag);
            Assert.True(zeroFlag);
            Assert.False(negativeFlag);
        }

        [Fact]
        public void NoOutput() {
            var noData = BitArrayHelper.FromString("00000000");
            var dataA = BitArrayHelper.FromString("11001111");
            var dataB = BitArrayHelper.FromString("00000001");

            var addResult = BitArrayHelper.FromString("11001110");

            using var adder = CreateAdder(out var inputs, out var outputs, out var outputEnable, out var a, out var b, out var c, out var carryIn, out var carryOut, out var negativeFlag, out var zeroFlag, out var overflowFlag);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = dataA[i];
                inputs[i + 8].Value = dataB[i];
            }

            c.Value = true;

            MakeTickAndWait();

            Assert.True(outputs.ToBitArray().EqualTo(noData));
        }

        [Fact]
        public void Not() {
            var dataA = BitArrayHelper.FromString("10101100");
            var dataB = BitArrayHelper.FromString("11001111");

            var notAddResult = BitArrayHelper.FromString("10000100");

            using var adder = CreateAdder(out var inputs, out var outputs, out var outputEnable, out var a, out var b, out var c, out var carryIn, out var carryOut, out var negativeFlag, out var zeroFlag, out var overflowFlag);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = dataA[i];
                inputs[i + 8].Value = dataB[i];
            }

            outputEnable.Value = true;
            b.Value = true;

            MakeTickAndWait();

            BitAssert.Equality(notAddResult, adder.Content);
            BitAssert.Equality(notAddResult, outputs);
            Assert.True(carryOut);
            Assert.False(overflowFlag);
            Assert.False(zeroFlag);
            Assert.True(negativeFlag);
        }

        [Fact]
        public void Or() {
            var dataA = BitArrayHelper.FromString("10101100");
            var dataB = BitArrayHelper.FromString("11001111");

            var orResult = BitArrayHelper.FromString("11101111");

            using var adder = CreateAdder(out var inputs, out var outputs, out var outputEnable, out var a, out var b, out var c, out var carryIn, out var carryOut, out var negativeFlag, out var zeroFlag, out var overflowFlag);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = dataA[i];
                inputs[i + 8].Value = dataB[i];
            }

            outputEnable.Value = true;
            b.Value = true;
            c.Value = true;

            MakeTickAndWait();

            BitAssert.Equality(orResult, adder.Content);
            BitAssert.Equality(orResult, outputs);
            Assert.False(carryOut);
            Assert.False(overflowFlag);
            Assert.False(zeroFlag);
            Assert.True(negativeFlag);
        }

        [Fact]
        public void And() {
            var dataA = BitArrayHelper.FromString("10101100");
            var dataB = BitArrayHelper.FromString("11001111");

            var andResult = BitArrayHelper.FromString("10001100");

            using var adder = CreateAdder(out var inputs, out var outputs, out var outputEnable, out var a, out var b, out var c, out var carryIn, out var carryOut, out var negativeFlag, out var zeroFlag, out var overflowFlag);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = dataA[i];
                inputs[i + 8].Value = dataB[i];
            }

            outputEnable.Value = true;
            a.Value = true;

            MakeTickAndWait();

            BitAssert.Equality(andResult, adder.Content);
            BitAssert.Equality(andResult, outputs);
            Assert.False(carryOut);
            Assert.False(overflowFlag);
            Assert.False(zeroFlag);
            Assert.True(negativeFlag);
        }

        [Fact]
        public void Xor() {
            var dataA = BitArrayHelper.FromString("10101100");
            var dataB = BitArrayHelper.FromString("11001111");

            var xorResult = BitArrayHelper.FromString("01100011");

            using var adder = CreateAdder(out var inputs, out var outputs, out var outputEnable, out var a, out var b, out var c, out var carryIn, out var carryOut, out var negativeFlag, out var zeroFlag, out var overflowFlag);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = dataA[i];
                inputs[i + 8].Value = dataB[i];
            }

            outputEnable.Value = true;
            a.Value = true;
            c.Value = true;

            MakeTickAndWait();

            BitAssert.Equality(xorResult, adder.Content);
            BitAssert.Equality(xorResult, outputs);
            Assert.False(carryOut);
            Assert.False(overflowFlag);
            Assert.False(zeroFlag);
            Assert.False(negativeFlag);
        }

        [Fact]
        public void Sl() {
            var dataA = BitArrayHelper.FromString("10101100");
            var dataB = BitArrayHelper.FromString("11001111");

            var slResult = BitArrayHelper.FromString("11110110");

            using var adder = CreateAdder(out var inputs, out var outputs, out var outputEnable, out var a, out var b, out var c, out var carryIn, out var carryOut, out var negativeFlag, out var zeroFlag, out var overflowFlag);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = dataA[i];
                inputs[i + 8].Value = dataB[i];
            }

            outputEnable.Value = true;
            a.Value = true;
            b.Value = true;

            MakeTickAndWait();

            BitAssert.Equality(slResult, adder.Content);
            BitAssert.Equality(slResult, outputs);
            Assert.True(carryOut);
            Assert.False(overflowFlag);
            Assert.False(zeroFlag);
            Assert.True(negativeFlag);
        }

        [Fact]
        public void Sr() {
            var dataA = BitArrayHelper.FromString("10101100");
            var dataB = BitArrayHelper.FromString("11001111");

            var srResult = BitArrayHelper.FromString("00111101");

            using var adder = CreateAdder(out var inputs, out var outputs, out var outputEnable, out var a, out var b, out var c, out var carryIn, out var carryOut, out var negativeFlag, out var zeroFlag, out var overflowFlag);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = dataA[i];
                inputs[i + 8].Value = dataB[i];
            }

            outputEnable.Value = true;
            a.Value = true;
            b.Value = true;
            c.Value = true;

            MakeTickAndWait();

            BitAssert.Equality(srResult, adder.Content);
            BitAssert.Equality(srResult, outputs);
            Assert.True(carryOut);
            Assert.False(overflowFlag);
            Assert.False(zeroFlag);
            Assert.False(negativeFlag);
        }

        private HLAdder CreateAdder(out Signal[] inputs, out Signal[] outputs, out Signal outputEnable, out Signal a, out Signal b, out Signal c, out Signal carryIn,
                out Signal carryOut, out Signal negativeFlag, out Signal zeroFlag, out Signal overflowFlag) {
            var register = new HLAdder("adder", 8);

            inputs = register.CreateSignalAndPlugInInputs().ToArray();
            outputs = register.CreateSignalAndPlugInOutputs().ToArray();

            outputEnable = register.CreateSignalAndPlugInPort(r => r.OutputEnable);
            a = register.CreateSignalAndPlugInPort(r => r.A);
            b = register.CreateSignalAndPlugInPort(r => r.B);
            c = register.CreateSignalAndPlugInPort(r => r.C);
            carryIn = register.CreateSignalAndPlugInPort(r => r.CarryIn);
            carryOut = register.CreateSignalAndPlugInPort(r => r.CarryFlag);
            negativeFlag = register.CreateSignalAndPlugInPort(r => r.NegativeFlag);
            overflowFlag = register.CreateSignalAndPlugInPort(r => r.OverflowFlag);
            zeroFlag = register.CreateSignalAndPlugInPort(r => r.ZeroFlag);

            return register;
        }
    }
}

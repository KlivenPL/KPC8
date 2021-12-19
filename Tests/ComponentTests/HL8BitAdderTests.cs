﻿using _Infrastructure.BitArrays;
using Components.Adders;
using Components.Signals;
using Infrastructure.BitArrays;
using System.Linq;
using Tests.Adapters;
using Xunit;

namespace Tests.ComponentTests {
    public class HL8BitAdderTests : TestBase {

        [Fact]
        public void LoadAddAndOutput() {
            var dataA = BitArrayHelper.FromString("10101100");
            var dataB = BitArrayHelper.FromString("11001111");

            var addResult = BitArrayHelper.FromString("01111100");

            var adder = CreateAdder(out var inputs, out var outputs, out var outputEnable, out var substractEnable, out var carryIn, out var carryOut);

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
        }

        [Fact]
        public void AddNoCarryOut() {
            var dataA = BitArrayHelper.FromString("10101100");
            var dataB = BitArrayHelper.FromString("00001111");

            var addResult = BitArrayHelper.FromString("10111011");

            var adder = CreateAdder(out var inputs, out var outputs, out var outputEnable, out var substractEnable, out var carryIn, out var carryOut);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = dataA[i];
                inputs[i + 8].Value = dataB[i];
            }

            outputEnable.Value = true;

            MakeTickAndWait();

            Assert.True(adder.Content.EqualTo(addResult));
            Assert.True(outputs.ToBitArray().EqualTo(addResult));
            Assert.False(carryOut);
        }


        [Fact]
        public void Substract() {
            var dataA = BitArrayHelper.FromString("11001111");
            var dataB = BitArrayHelper.FromString("10101100");

            var addResult = BitArrayHelper.FromString("00100011");

            var adder = CreateAdder(out var inputs, out var outputs, out var outputEnable, out var substractEnable, out var carryIn, out var carryOut);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = dataA[i];
                inputs[i + 8].Value = dataB[i];
            }

            outputEnable.Value = true;
            substractEnable.Value = true;

            MakeTickAndWait();

            Assert.True(adder.Content.EqualTo(addResult));
            Assert.True(outputs.ToBitArray().EqualTo(addResult));
            Assert.True(carryOut);
        }

        [Fact]
        public void NoOutput() {
            var noData = BitArrayHelper.FromString("00000000");
            var dataA = BitArrayHelper.FromString("11001111");
            var dataB = BitArrayHelper.FromString("00000001");

            var addResult = BitArrayHelper.FromString("11001110");

            var adder = CreateAdder(out var inputs, out var outputs, out var outputEnable, out var substractEnable, out var carryIn, out var carryOut);

            for (int i = 0; i < 8; i++) {
                inputs[i].Value = dataA[i];
                inputs[i + 8].Value = dataB[i];
            }

            substractEnable.Value = true;

            MakeTickAndWait();

            Assert.True(adder.Content.EqualTo(addResult));
            Assert.True(outputs.ToBitArray().EqualTo(noData));
            Assert.True(carryOut);
        }

        private HL8BitAdder CreateAdder(out Signal[] inputs, out Signal[] outputs, out Signal outputEnable, out Signal substractEnable, out Signal carryIn, out Signal carryOut) {
            var register = new HL8BitAdder();

            inputs = register.CreateSignalAndPlugInInputs().ToArray();
            outputs = register.CreateSignalAndPlugInOutputs().ToArray();

            outputEnable = register.CreateSignalAndPlugInPort(r => r.OutputEnable);
            substractEnable = register.CreateSignalAndPlugInPort(r => r.SubstractEnable);
            carryIn = register.CreateSignalAndPlugInPort(r => r.CarryIn);
            carryOut = register.CreateSignalAndPlugInPort(r => r.CarryOut);

            return register;
        }
    }
}

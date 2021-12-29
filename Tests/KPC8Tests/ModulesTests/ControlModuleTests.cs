using _Infrastructure.Collections;
using Components._Infrastructure.Components;
using Components.Buses;
using Infrastructure.BitArrays;
using KPC8.ControlSignals;
using KPC8.Modules;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tests._Infrastructure;
using Xunit;

namespace Tests.KPC8Tests.ModulesTests {
    public class ControlModuleTests : TestBase {

        [Fact]
        public void LoadInstructionToIr() {
            var instrHi = BitArrayHelper.FromString("11010001");
            var instrLo = BitArrayHelper.FromString("10100010");
            var totalInstruction = BitArrayHelper.FromString("11010001 10100010");

            var module = CreateControlModule(out var dataBus, out var registerSelectBus, out var controlBus, out var cs);

            dataBus.Write(instrHi);
            Enable(cs.Ir_le_hi);
            MakeTickAndWait();

            dataBus.Write(instrLo);
            Enable(cs.Ir_le_lo);
            MakeTickAndWait();

            BitAssert.Equality(totalInstruction, module.IrOutput);
        }

        [Theory]
        [InlineData("000000")]
        [InlineData("000001")]
        [InlineData("111111")]
        public void LoadOpcodeToInstRom(string opCode) {
            var instrHi = BitArrayHelper.FromString($"{opCode}00");
            var instrLo = BitArrayHelper.FromString("00000000");
            var instRomContent = BitArrayHelper.FromString($"{opCode}0000");

            var module = CreateControlModule(out var dataBus, out var registerSelectBus, out var controlBus, out var cs);

            dataBus.Write(instrHi);
            Enable(cs.Ir_le_hi);
            MakeTickAndWait();

            dataBus.Write(instrLo);
            Enable(cs.Ir_le_lo);
            Enable(cs.Ic_clr);
            MakeTickAndWait();

            BitAssert.Equality(instRomContent, module.InstRomAddress);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void InstRomOutputsInstructionsTest(int instructionOffset) {
            var instrHi = BitArrayHelper.FromString($"00000{instructionOffset}00");
            var instrLo = BitArrayHelper.FromString("00000000");

            var testRomData = CreateTestInstRomData().ToArray();

            var module = CreateControlModuleWithMockControlBus(testRomData, out var dataBus, out var registerSelectBus, out var controlBus, out var mockControlBus, out var cs);

            dataBus.Write(instrHi);
            Enable(cs.Ir_le_hi);
            MakeTickAndWait();

            dataBus.Write(instrLo);
            Enable(cs.Ir_le_lo);
            Enable(cs.Ic_clr);
            MakeTickAndWait();

            int startOffset = 16 * instructionOffset;
            for (int i = startOffset; i < startOffset + 37; i++) {
                BitAssert.Equality(testRomData[startOffset + i % 16], mockControlBus.Lanes);
                MakeTickAndWait();
            }
        }

        [Fact]
        public void DecodeAllProgrammerRegisters() {
            var instrHi = BitArrayHelper.FromString("00000100");
            var instrLo = BitArrayHelper.FromString("01001000");
            var totalInstruction = BitArrayHelper.FromString("00000100 01001000");

            var decodedDecDest = BitArrayHelper.FromString("00001000 00000000");
            var decodedDecA = BitArrayHelper.FromString("00001000 00000000");
            var decodedDecB = BitArrayHelper.FromString("00000000 10000000");

            var module = CreateControlModule(out var dataBus, out var registerSelectBus, out var controlBus, out var cs);

            dataBus.Write(instrHi);
            Enable(cs.Ir_le_hi);
            MakeTickAndWait();

            dataBus.Write(instrLo);
            Enable(cs.Ir_le_lo);
            MakeTickAndWait();

            BitAssert.Equality(totalInstruction, module.IrOutput);
            BitAssert.Equality(totalInstruction.Slice(6, 2), module.DecDestInput.Skip(2));
            BitAssert.Equality(totalInstruction.Slice(8, 4), module.DecAInput);
            BitAssert.Equality(totalInstruction.Slice(12, 4), module.DecBInput);

            Enable(cs.DecA_oe);
            MakeTickAndWait();
            BitAssert.Equality(decodedDecA, registerSelectBus.Lanes);

            Enable(cs.DecB_oe);
            MakeTickAndWait();
            BitAssert.Equality(decodedDecB, registerSelectBus.Lanes);

            Enable(cs.DecDest_oe);
            MakeTickAndWait();
            BitAssert.Equality(decodedDecDest, registerSelectBus.Lanes);
        }

        [Theory]
        [InlineData("00")]
        [InlineData("01")]
        [InlineData("10")]
        [InlineData("11")]
        public void DecodeDecReg(string decRegEncoded) {
            var instrHi = BitArrayHelper.FromString($"000000{decRegEncoded}");
            var instrLo = BitArrayHelper.FromString("00000000");
            var totalInstruction = instrHi.MergeWith(instrLo);

            var decodedDecDest = new BitArray(16);
            decodedDecDest.Set(BitArrayHelper.FromString($"01{decRegEncoded}").ToIntLE(), true);

            var module = CreateControlModule(out var dataBus, out var registerSelectBus, out var controlBus, out var cs);

            dataBus.Write(instrHi);
            Enable(cs.Ir_le_hi);
            MakeTickAndWait();

            dataBus.Write(instrLo);
            Enable(cs.Ir_le_lo);
            MakeTickAndWait();

            BitAssert.Equality(totalInstruction.Slice(6, 2), module.DecDestInput.Skip(2));

            Enable(cs.DecDest_oe);
            MakeTickAndWait();
            BitAssert.Equality(decodedDecDest, registerSelectBus.Lanes);
        }

        [Theory]
        [InlineData("0000")]
        [InlineData("0001")]
        [InlineData("0010")]
        [InlineData("0011")]
        [InlineData("0100")]
        [InlineData("0101")]
        [InlineData("0110")]
        [InlineData("0111")]
        [InlineData("1000")]
        [InlineData("1001")]
        [InlineData("1010")]
        [InlineData("1011")]
        [InlineData("1100")]
        [InlineData("1101")]
        [InlineData("1110")]
        [InlineData("1111")]
        public void DecodeAandBRegs(string regEncoded) {
            var zero = BitArrayHelper.FromString("00000000 00000000");
            var instrHi = BitArrayHelper.FromString("00000000");
            var instrLo = BitArrayHelper.FromString($"{regEncoded}{regEncoded}");
            var totalInstruction = instrHi.MergeWith(instrLo);

            var decodedReg = new BitArray(16);
            decodedReg.Set(BitArrayHelper.FromString(regEncoded).ToIntLE(), true);

            var module = CreateControlModule(out var dataBus, out var registerSelectBus, out var controlBus, out var cs);

            dataBus.Write(instrHi);
            Enable(cs.Ir_le_hi);
            MakeTickAndWait();

            dataBus.Write(instrLo);
            Enable(cs.Ir_le_lo);
            MakeTickAndWait();

            BitAssert.Equality(totalInstruction.Slice(8, 4), module.DecAInput);
            BitAssert.Equality(totalInstruction.Slice(12, 4), module.DecBInput);

            Enable(cs.DecA_oe);
            MakeTickAndWait();
            BitAssert.Equality(decodedReg, registerSelectBus.Lanes);

            registerSelectBus.Write(zero);
            MakeTickAndWait();
            BitAssert.Equality(zero, registerSelectBus.Lanes);

            Enable(cs.DecB_oe);
            MakeTickAndWait();
            BitAssert.Equality(decodedReg, registerSelectBus.Lanes);
        }

        private Control CreateControlModule(out IBus dataBus, out IBus registerSelectBus, out IBus controlBus, out CsPanel.ControlPanel csPanel) {
            dataBus = new HLBus("TestDataBus", 8);
            registerSelectBus = new HLBus("RegisterSelectBus", 16);
            controlBus = new HLBus("ControlBus", 32);

            var control = new Control(null, _testClock, dataBus, registerSelectBus);
            csPanel = control.CreateControlPanel(controlBus);

            return control;
        }

        private Control CreateControlModuleWithMockControlBus(BitArray[] romData, out IBus dataBus, out IBus registerSelectBus, out IBus controlBus, out IBus mockControlBus, out CsPanel.ControlPanel csPanel) {
            dataBus = new HLBus("TestDataBus", 8);
            registerSelectBus = new HLBus("RegisterSelectBus", 16);
            controlBus = new HLBus("ControlBus", 32);
            mockControlBus = new HLBus("MockControlBus", 32);

            var control = new Control(romData, _testClock, dataBus, registerSelectBus);
            csPanel = control.CreateControlPanel(controlBus);
            control.ConnectControlBusToControllerPorts(mockControlBus);

            return control;
        }

        private IEnumerable<BitArray> CreateTestInstRomData() {
            for (int i = 0; i < 3; i++) {
                foreach (var cs in Enum.GetValues<ControlSignalType>().Skip(1).Shuffle()) {
                    yield return cs.ToBitArray();
                }
            }
        }
    }
}

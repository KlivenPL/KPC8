using _Infrastructure.Collections;
using Components._Infrastructure.Components;
using Components.Buses;
using Infrastructure.BitArrays;
using KPC8.ControlSignals;
using KPC8.Modules;
using KPC8.ProgRegs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tests._Infrastructure;
using Xunit;

namespace Tests.KPC8Tests.Modules {
    public class ControlModuleTests : TestBase {

        [Fact]
        public void LoadInstructionToIr() {
            var instrHi = BitArrayHelper.FromString("11010001");
            var instrLo = BitArrayHelper.FromString("10100010");
            var totalInstruction = BitArrayHelper.FromString("11010001 10100010");

            var module = CreateControlModule(out var dataBus, out var registerSelectBus, out var controlBus, out var flagsBus, out var cs);

            dataBus.Write(instrHi);
            Enable(cs.Ir_le_hi);
            MakeTickAndWait();

            dataBus.Write(instrLo);
            Enable(cs.Ir_le_lo);
            MakeTickAndWait();

            BitAssert.Equality(totalInstruction, module.IrOutput);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(9)]
        [InlineData(15)]
        public void LoadInstructionToIr_Interrupt_InterruptLoadedInstead(byte romInterruptCode) {
            var instrHiIgnored = BitArrayHelper.FromString("11111111");
            var instrLoIgnored = BitArrayHelper.FromString("11111111");

            var interruptCode = BitArrayHelper.FromByteLE(romInterruptCode).Skip(4);

            var module = CreateControlModuleWithInterrupts(out var dataBus, out var registerSelectBus, out var controlBus, out var interruptsBus, out var cs);

            dataBus.Write(instrHiIgnored);

            interruptsBus.Lanes[0].Value = true;
            interruptsBus.Write(4, interruptCode);
            Enable(cs.Irr_b);
            MakeTickAndWait();

            Enable(cs.Ic_clr);
            MakeTickAndWait();

            Enable(cs.Ir_le_hi);
            MakeTickAndWait();

            Assert.True(module.GetIrrSignal(irr => irr.ShouldProcessInterrupt));
            BitAssert.Equality(interruptCode, module.IrrRomAddress);

            dataBus.Write(instrLoIgnored);
            Enable(cs.Ir_le_lo);
            MakeTickAndWait();

            Assert.True(module.GetIrrSignal(irr => irr.ShouldProcessInterrupt));
            BitAssert.Equality(interruptCode, module.IrrRomAddress);

            BitAssert.Equality(module.GetIrrExpectedRomData(romInterruptCode), module.IrOutput);
        }

        [Theory]
        [InlineData("000000", "00000000000")] // Procedural instruction
        [InlineData("000001", "00000010000")] // Procedural instruction
        [InlineData("011101", "00111010000")] // Procedural instruction
        [InlineData("111000", "10000000000")] // Conditional instruction
        [InlineData("111010", "10100000000")] // Conditional instruction
        [InlineData("111111", "11110000000")] // Conditional instruction
        public void LoadOpcodeToInstRom(string opCode, string instRomContentStr) {
            var instrHi = BitArrayHelper.FromString($"{opCode}00");
            var instrLo = BitArrayHelper.FromString("00000000");
            var instRomContent = BitArrayHelper.FromString(instRomContentStr);

            var module = CreateControlModule(out var dataBus, out var registerSelectBus, out var controlBus, out var flagsBus, out var cs);

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
        [InlineData("000000")] // Procedural instruction opcode
        [InlineData("001010")] // Procedural instruction opcode
        [InlineData("110111")] // Procedural instruction opcode
        public void LoadProceduralInstructionToInstRom(string opCode) {
            var instrHi = BitArrayHelper.FromString($"{opCode}00");
            var instrLo = BitArrayHelper.FromString("00000000");
            var flags = BitArrayHelper.FromString("1101");

            var module = CreateControlModule(out var dataBus, out var registerSelectBus, out var controlBus, out var flagsBus, out var cs);
            flagsBus.Write(flags);

            dataBus.Write(instrHi);
            Enable(cs.Ir_le_hi);
            MakeTickAndWait();

            dataBus.Write(instrLo);
            Enable(cs.Ir_le_lo);
            MakeTickAndWait();

            var instRomContent = BitArrayHelper.FromString($"0{opCode}{module.IcOutput.ToBitString()}");
            BitAssert.Equality(instRomContent, module.InstRomAddress);
        }

        [Theory]
        [InlineData("111000")] // Conditional instruction opcode
        [InlineData("111010")] // Conditional instruction opcode
        [InlineData("111111")] // Conditional instruction opcode
        public void LoadConditionalInstructionToInstRom(string opCode) {
            var instrHi = BitArrayHelper.FromString($"{opCode}00");
            var instrLo = BitArrayHelper.FromString("00000000");
            var flags = BitArrayHelper.FromString("1101");

            var module = CreateControlModule(out var dataBus, out var registerSelectBus, out var controlBus, out var flagsBus, out var cs);
            flagsBus.Write(flags);

            dataBus.Write(instrHi);
            Enable(cs.Ir_le_hi);
            MakeTickAndWait();

            dataBus.Write(instrLo);
            Enable(cs.Ir_le_lo);
            MakeTickAndWait();

            var instRomContent = BitArrayHelper.FromString($"1{opCode.Substring(3, 3)}{flagsBus.PeakAll().ToBitString()}{module.IcOutput.Skip(1).ToBitString()}");
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
        public void Output8LSBFromIrToDataBus() {
            var zero = BitArrayHelper.FromString("00000000");
            var instrHi = BitArrayHelper.FromString("11010001");
            var instrLo = BitArrayHelper.FromString("10100010");
            var totalInstruction = BitArrayHelper.FromString("11010001 10100010");

            var module = CreateControlModule(out var dataBus, out var registerSelectBus, out var controlBus, out var flagsBus, out var cs);

            dataBus.Write(instrHi);
            Enable(cs.Ir_le_hi);
            MakeTickAndWait();

            dataBus.Write(instrLo);
            Enable(cs.Ir_le_lo);
            MakeTickAndWait();

            dataBus.Write(zero);
            Enable(cs.Ir8LSBToBus_oe);
            MakeTickAndWait();

            BitAssert.Equality(instrLo, dataBus.Lanes);
        }

        [Fact]
        public void DecodeAllProgrammerRegisters() {
            var instrHi = BitArrayHelper.FromString("00000100");
            var instrLo = BitArrayHelper.FromString("01001000");
            var totalInstruction = BitArrayHelper.FromString("00000100 01001000");

            var decodedDecDest = BitArrayHelper.FromString("00001000 00000000");
            var decodedDecA = BitArrayHelper.FromString("00001000 00000000");
            var decodedDecB = BitArrayHelper.FromString("00000000 10000000");

            var module = CreateControlModule(out var dataBus, out var registerSelectBus, out var controlBus, out var flagsBus, out var cs);

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
            decodedDecDest.Set(BitArrayHelper.FromString($"01{decRegEncoded}").GetSignedValueLE(), true);

            var module = CreateControlModule(out var dataBus, out var registerSelectBus, out var controlBus, out var flagsBus, out var cs);

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
        [InlineData(Regs.Zero)]
        [InlineData(Regs.Ass)]
        [InlineData(Regs.Sp)]
        [InlineData(Regs.Fp)]
        [InlineData(Regs.T1)]
        [InlineData(Regs.T2)]
        [InlineData(Regs.T3)]
        [InlineData(Regs.T4)]
        [InlineData(Regs.S1)]
        [InlineData(Regs.S2)]
        [InlineData(Regs.S3)]
        [InlineData(Regs.A1)]
        [InlineData(Regs.A2)]
        [InlineData(Regs.A3)]
        [InlineData(Regs.Rt)]
        [InlineData(Regs.Ra)]
        public void DecodeAandBRegs(Regs reg) {
            var zero = BitArrayHelper.FromString("00000000 00000000");
            var instrHi = BitArrayHelper.FromString("00000000");
            var instrLo = BitArrayHelper.FromString($"{reg.GetEncodedAddress().ToBitString()}{reg.GetEncodedAddress().ToBitString()}");
            var totalInstruction = instrHi.MergeWith(instrLo);

            var decodedReg = reg.GetDecodedAddress();

            var module = CreateControlModule(out var dataBus, out var registerSelectBus, out var controlBus, out var flagsBus, out var cs);

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

        private Control CreateControlModule(out IBus dataBus, out IBus registerSelectBus, out IBus controlBus, out IBus flagsBus, out CsPanel.ControlPanel csPanel) {
            dataBus = new HLBus("TestDataBus", 8);
            registerSelectBus = new HLBus("RegisterSelectBus", 16);
            controlBus = new HLBus("ControlBus", 40);
            flagsBus = new HLBus("FlagsBus", 4);

            var control = new Control(null, _testClock.Clk, dataBus, registerSelectBus, flagsBus, new HLBus("InterruptsBus", 8));
            csPanel = control.CreateControlPanel(controlBus);

            return control;
        }

        private Control CreateControlModuleWithInterrupts(out IBus dataBus, out IBus registerSelectBus, out IBus controlBus, out IBus interruptsBus, out CsPanel.ControlPanel csPanel) {
            dataBus = new HLBus("TestDataBus", 8);
            registerSelectBus = new HLBus("RegisterSelectBus", 16);
            controlBus = new HLBus("ControlBus", 40);
            interruptsBus = new HLBus("InterruptsBus", 8);

            var control = new Control(null, _testClock.Clk, dataBus, registerSelectBus, new HLBus("FlagsBus", 4), interruptsBus);
            csPanel = control.CreateControlPanel(controlBus);

            return control;
        }

        private Control CreateControlModuleWithMockControlBus(BitArray[] romData, out IBus dataBus, out IBus registerSelectBus, out IBus controlBus, out IBus mockControlBus, out CsPanel.ControlPanel csPanel) {
            dataBus = new HLBus("TestDataBus", 8);
            registerSelectBus = new HLBus("RegisterSelectBus", 16);
            controlBus = new HLBus("ControlBus", 40);
            mockControlBus = new HLBus("MockControlBus", 40);

            var control = new Control(romData, _testClock.Clk, dataBus, registerSelectBus, new HLBus("Flags bus", 4), new HLBus("InterruptsBus", 8));
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

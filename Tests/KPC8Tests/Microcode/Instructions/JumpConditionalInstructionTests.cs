using Infrastructure.BitArrays;
using KPC8.Microcode;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections;
using Tests._Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Tests.KPC8Tests.Microcode.Instructions {
    public class JumpConditionalInstructionTests : McInstructionTestBase {
        public JumpConditionalInstructionTests(ITestOutputHelper debug) : base(debug) {

        }

        [Theory]
        [InlineData(0x2137, 0xFF00)]
        [InlineData(0x0, 0x233)]
        [InlineData(0x911, 0x2137)]
        [InlineData(0x0, 0xFF00)]
        public void Jwz(ushort valueStr, ushort addrStr) {
            var instruction = McConditionalInstruction.CreateFromSteps(typeof(JumpConditionalInstructions), nameof(JumpConditionalInstructions.Jwz));
            var positiveTestInstr = McProceduralInstruction.CreateFromSteps(typeof(RegsProceduralInstructions), nameof(RegsProceduralInstructions.SetI));
            var negativeTestInstr = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.AddI));

            var value = BitArrayHelper.FromUShortLE(valueStr);
            var addr = BitArrayHelper.FromUShortLE(addrStr);
            var one = BitArrayHelper.FromByteLE(231);
            var five = BitArrayHelper.FromByteLE(5);
            var skipAddress = BitArrayHelper.FromShortLE(2);

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh, out var instructionLow);
            EncodeInstruction(positiveTestInstr, Regs.T4, one, out var positiveTestInstrHigh, out var positiveTestInstrLow);
            EncodeInstruction(negativeTestInstr, Regs.T4, five, out var negativeTestInstrHigh, out var negativeTestInstrLow);

            var romData = new BitArray[0xFFFF + 1];
            romData[0] = instructionHigh;
            romData[1] = instructionLow;

            romData[2] = negativeTestInstrHigh;
            romData[3] = negativeTestInstrLow;

            romData[addrStr] = positiveTestInstrHigh;
            romData[addrStr + 1] = positiveTestInstrLow;

            var cp = BuildPcModules(romData, out var modules);

            // var instRomDump = string.Join("\r\n", modules.Control.DumpInstrRomToControlSignals());

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), value);
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);

            StepThroughConditionalInstruction(modules, instruction);

            BitAssert.Equality(value, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));

            if (valueStr == 0) {
                BitAssert.Equality(addr, modules.Memory.PcContent);
                StepThroughProceduralInstruction(modules, positiveTestInstr);

                BitAssert.Equality(one, modules.Registers.GetLoRegContent(Regs.T4.GetIndex()));
            } else {
                BitAssert.Equality(skipAddress, modules.Memory.PcContent);
                StepThroughProceduralInstruction(modules, negativeTestInstr);

                BitAssert.Equality(five, modules.Registers.GetLoRegContent(Regs.T4.GetIndex()));
            }
        }

        [Theory]
        [InlineData(0x2137, 0xFF00)]
        [InlineData(0x0, 0x233)]
        [InlineData(0x911, 0x2137)]
        [InlineData(0x0, 0xFF00)]
        public void Jwnotz(ushort valueStr, ushort addrStr) {
            var instruction = McConditionalInstruction.CreateFromSteps(typeof(JumpConditionalInstructions), nameof(JumpConditionalInstructions.Jwnotz));
            var positiveTestInstr = McProceduralInstruction.CreateFromSteps(typeof(RegsProceduralInstructions), nameof(RegsProceduralInstructions.SetI));
            var negativeTestInstr = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.AddI));

            var value = BitArrayHelper.FromUShortLE(valueStr);
            var addr = BitArrayHelper.FromUShortLE(addrStr);
            var one = BitArrayHelper.FromByteLE(231);
            var five = BitArrayHelper.FromByteLE(5);
            var skipAddress = BitArrayHelper.FromShortLE(2);

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh, out var instructionLow);
            EncodeInstruction(positiveTestInstr, Regs.T4, one, out var positiveTestInstrHigh, out var positiveTestInstrLow);
            EncodeInstruction(negativeTestInstr, Regs.T4, five, out var negativeTestInstrHigh, out var negativeTestInstrLow);

            var romData = new BitArray[0xFFFF + 1];
            romData[0] = instructionHigh;
            romData[1] = instructionLow;

            romData[2] = negativeTestInstrHigh;
            romData[3] = negativeTestInstrLow;

            romData[addrStr] = positiveTestInstrHigh;
            romData[addrStr + 1] = positiveTestInstrLow;

            var cp = BuildPcModules(romData, out var modules);

            // var instRomDump = string.Join("\r\n", modules.Control.DumpInstrRomToControlSignals());

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), value);
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);

            StepThroughConditionalInstruction(modules, instruction);

            BitAssert.Equality(value, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));

            if (valueStr != 0) {
                BitAssert.Equality(addr, modules.Memory.PcContent);
                StepThroughProceduralInstruction(modules, positiveTestInstr);

                BitAssert.Equality(one, modules.Registers.GetLoRegContent(Regs.T4.GetIndex()));
            } else {
                BitAssert.Equality(skipAddress, modules.Memory.PcContent);
                StepThroughProceduralInstruction(modules, negativeTestInstr);

                BitAssert.Equality(five, modules.Registers.GetLoRegContent(Regs.T4.GetIndex()));
            }
        }

        [Theory]
        [InlineData(-2137, 0xFF00)]
        [InlineData(15, 0x233)]
        [InlineData(-15, 0x2137)]
        [InlineData(2137, 0xFF00)]
        [InlineData(0, 0xFF00)]
        public void Jwn(short valueStr, ushort addrStr) {
            var instruction = McConditionalInstruction.CreateFromSteps(typeof(JumpConditionalInstructions), nameof(JumpConditionalInstructions.Jwn));
            var positiveTestInstr = McProceduralInstruction.CreateFromSteps(typeof(RegsProceduralInstructions), nameof(RegsProceduralInstructions.SetI));
            var negativeTestInstr = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.AddI));

            var value = BitArrayHelper.FromShortLE(valueStr);
            var addr = BitArrayHelper.FromUShortLE(addrStr);
            var one = BitArrayHelper.FromByteLE(231);
            var five = BitArrayHelper.FromByteLE(5);
            var skipAddress = BitArrayHelper.FromShortLE(2);

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh, out var instructionLow);
            EncodeInstruction(positiveTestInstr, Regs.T4, one, out var positiveTestInstrHigh, out var positiveTestInstrLow);
            EncodeInstruction(negativeTestInstr, Regs.T4, five, out var negativeTestInstrHigh, out var negativeTestInstrLow);

            var romData = new BitArray[0xFFFF + 1];
            romData[0] = instructionHigh;
            romData[1] = instructionLow;

            romData[2] = negativeTestInstrHigh;
            romData[3] = negativeTestInstrLow;

            romData[addrStr] = positiveTestInstrHigh;
            romData[addrStr + 1] = positiveTestInstrLow;

            var cp = BuildPcModules(romData, out var modules);

            // var instRomDump = string.Join("\r\n", modules.Control.DumpInstrRomToControlSignals());

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), value);
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);

            StepThroughConditionalInstruction(modules, instruction);

            BitAssert.Equality(value, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));

            if (valueStr < 0) {
                BitAssert.Equality(addr, modules.Memory.PcContent);
                StepThroughProceduralInstruction(modules, positiveTestInstr);

                BitAssert.Equality(one, modules.Registers.GetLoRegContent(Regs.T4.GetIndex()));
            } else {
                BitAssert.Equality(skipAddress, modules.Memory.PcContent);
                StepThroughProceduralInstruction(modules, negativeTestInstr);

                BitAssert.Equality(five, modules.Registers.GetLoRegContent(Regs.T4.GetIndex()));
            }
        }

        [Theory]
        [InlineData(-2137, 0xFF00)]
        [InlineData(15, 0x233)]
        [InlineData(-15, 0x2137)]
        [InlineData(2137, 0xFF00)]
        [InlineData(0, 0xFF00)]
        public void Jwnotn(short valueStr, ushort addrStr) {
            var instruction = McConditionalInstruction.CreateFromSteps(typeof(JumpConditionalInstructions), nameof(JumpConditionalInstructions.Jwnotn));
            var positiveTestInstr = McProceduralInstruction.CreateFromSteps(typeof(RegsProceduralInstructions), nameof(RegsProceduralInstructions.SetI));
            var negativeTestInstr = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.AddI));

            var value = BitArrayHelper.FromShortLE(valueStr);
            var addr = BitArrayHelper.FromUShortLE(addrStr);
            var one = BitArrayHelper.FromByteLE(231);
            var five = BitArrayHelper.FromByteLE(5);
            var skipAddress = BitArrayHelper.FromShortLE(2);

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh, out var instructionLow);
            EncodeInstruction(positiveTestInstr, Regs.T4, one, out var positiveTestInstrHigh, out var positiveTestInstrLow);
            EncodeInstruction(negativeTestInstr, Regs.T4, five, out var negativeTestInstrHigh, out var negativeTestInstrLow);

            var romData = new BitArray[0xFFFF + 1];
            romData[0] = instructionHigh;
            romData[1] = instructionLow;

            romData[2] = negativeTestInstrHigh;
            romData[3] = negativeTestInstrLow;

            romData[addrStr] = positiveTestInstrHigh;
            romData[addrStr + 1] = positiveTestInstrLow;

            var cp = BuildPcModules(romData, out var modules);

            // var instRomDump = string.Join("\r\n", modules.Control.DumpInstrRomToControlSignals());

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), value);
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);

            StepThroughConditionalInstruction(modules, instruction);

            BitAssert.Equality(value, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));

            if (valueStr >= 0) {
                BitAssert.Equality(addr, modules.Memory.PcContent);
                StepThroughProceduralInstruction(modules, positiveTestInstr);

                BitAssert.Equality(one, modules.Registers.GetLoRegContent(Regs.T4.GetIndex()));
            } else {
                BitAssert.Equality(skipAddress, modules.Memory.PcContent);
                StepThroughProceduralInstruction(modules, negativeTestInstr);

                BitAssert.Equality(five, modules.Registers.GetLoRegContent(Regs.T4.GetIndex()));
            }
        }
    }
}

using Infrastructure.BitArrays;
using KPC8.Microcode;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections;
using Tests._Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Tests.KPC8Tests.Integration.Instructions {
    public class JumpConditionalInstructionTests : McInstructionTestBase {
        public JumpConditionalInstructionTests(ITestOutputHelper debug) : base(debug) {

        }

        [Theory]
        [InlineData(0x2137, 0xFF00)]
        [InlineData(0x0, 0x233)]
        [InlineData(0x911, 0x2137)]
        [InlineData(5, 0x2137)]
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
            EncodeInstruction(positiveTestInstr, Regs.Ass, one, out var positiveTestInstrHigh, out var positiveTestInstrLow);
            EncodeInstruction(negativeTestInstr, Regs.Ass, five, out var negativeTestInstrHigh, out var negativeTestInstrLow);

            var romData = new BitArray[0xFFFF + 1];
            romData[0] = instructionHigh;
            romData[1] = instructionLow;

            romData[2] = negativeTestInstrHigh;
            romData[3] = negativeTestInstrLow;

            romData[addrStr] = positiveTestInstrHigh;
            romData[addrStr + 1] = positiveTestInstrLow;

            var cp = BuildPcModules(romData, out var modules);
            var lw = BuildLwEmulator(romData, null);

            // var instRomDump = string.Join("\r\n", modules.Control.DumpInstrRomToControlSignals());

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), value);
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);

            CopyRegsToLw(lw, modules);
            EmuLwIntegrity.AssertFullIntegrity(lw, modules);

            StepThroughConditionalInstruction(modules, instruction);
            ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

            BitAssert.Equality(value, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));

            if (valueStr == 0) {
                BitAssert.Equality(addr, modules.Memory.PcContent);
                StepThroughProceduralInstruction(modules, positiveTestInstr);
                ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

                BitAssert.Equality(one, modules.Registers.GetLoRegContent(Regs.Ass.GetIndex()));
            } else {
                BitAssert.Equality(skipAddress, modules.Memory.PcContent);
                StepThroughProceduralInstruction(modules, negativeTestInstr);
                ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

                BitAssert.Equality(five, modules.Registers.GetLoRegContent(Regs.Ass.GetIndex()));
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
            EncodeInstruction(positiveTestInstr, Regs.Ass, one, out var positiveTestInstrHigh, out var positiveTestInstrLow);
            EncodeInstruction(negativeTestInstr, Regs.Ass, five, out var negativeTestInstrHigh, out var negativeTestInstrLow);

            var romData = new BitArray[0xFFFF + 1];
            romData[0] = instructionHigh;
            romData[1] = instructionLow;

            romData[2] = negativeTestInstrHigh;
            romData[3] = negativeTestInstrLow;

            romData[addrStr] = positiveTestInstrHigh;
            romData[addrStr + 1] = positiveTestInstrLow;

            var cp = BuildPcModules(romData, out var modules);
            var lw = BuildLwEmulator(romData, null);

            // var instRomDump = string.Join("\r\n", modules.Control.DumpInstrRomToControlSignals());

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), value);
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);

            CopyRegsToLw(lw, modules);
            EmuLwIntegrity.AssertFullIntegrity(lw, modules);

            StepThroughConditionalInstruction(modules, instruction);
            ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

            BitAssert.Equality(value, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));

            if (valueStr != 0) {
                BitAssert.Equality(addr, modules.Memory.PcContent);
                StepThroughProceduralInstruction(modules, positiveTestInstr);
                ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

                BitAssert.Equality(one, modules.Registers.GetLoRegContent(Regs.Ass.GetIndex()));
            } else {
                BitAssert.Equality(skipAddress, modules.Memory.PcContent);
                StepThroughProceduralInstruction(modules, negativeTestInstr);
                ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

                BitAssert.Equality(five, modules.Registers.GetLoRegContent(Regs.Ass.GetIndex()));
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
            EncodeInstruction(positiveTestInstr, Regs.Ass, one, out var positiveTestInstrHigh, out var positiveTestInstrLow);
            EncodeInstruction(negativeTestInstr, Regs.Ass, five, out var negativeTestInstrHigh, out var negativeTestInstrLow);

            var romData = new BitArray[0xFFFF + 1];
            romData[0] = instructionHigh;
            romData[1] = instructionLow;

            romData[2] = negativeTestInstrHigh;
            romData[3] = negativeTestInstrLow;

            romData[addrStr] = positiveTestInstrHigh;
            romData[addrStr + 1] = positiveTestInstrLow;

            var cp = BuildPcModules(romData, out var modules);
            var lw = BuildLwEmulator(romData, null);

            // var instRomDump = string.Join("\r\n", modules.Control.DumpInstrRomToControlSignals());

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), value);
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);

            CopyRegsToLw(lw, modules);
            EmuLwIntegrity.AssertFullIntegrity(lw, modules);

            StepThroughConditionalInstruction(modules, instruction);
            ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

            BitAssert.Equality(value, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));

            if (valueStr < 0) {
                BitAssert.Equality(addr, modules.Memory.PcContent);
                StepThroughProceduralInstruction(modules, positiveTestInstr);
                ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

                BitAssert.Equality(one, modules.Registers.GetLoRegContent(Regs.Ass.GetIndex()));
            } else {
                BitAssert.Equality(skipAddress, modules.Memory.PcContent);
                StepThroughProceduralInstruction(modules, negativeTestInstr);
                ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

                BitAssert.Equality(five, modules.Registers.GetLoRegContent(Regs.Ass.GetIndex()));
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
            EncodeInstruction(positiveTestInstr, Regs.Ass, one, out var positiveTestInstrHigh, out var positiveTestInstrLow);
            EncodeInstruction(negativeTestInstr, Regs.Ass, five, out var negativeTestInstrHigh, out var negativeTestInstrLow);

            var romData = new BitArray[0xFFFF + 1];
            romData[0] = instructionHigh;
            romData[1] = instructionLow;

            romData[2] = negativeTestInstrHigh;
            romData[3] = negativeTestInstrLow;

            romData[addrStr] = positiveTestInstrHigh;
            romData[addrStr + 1] = positiveTestInstrLow;

            var cp = BuildPcModules(romData, out var modules);
            var lw = BuildLwEmulator(romData, null);

            // var instRomDump = string.Join("\r\n", modules.Control.DumpInstrRomToControlSignals());

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), value);
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);

            CopyRegsToLw(lw, modules);
            EmuLwIntegrity.AssertFullIntegrity(lw, modules);

            StepThroughConditionalInstruction(modules, instruction);
            ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

            BitAssert.Equality(value, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));

            if (valueStr >= 0) {
                BitAssert.Equality(addr, modules.Memory.PcContent);
                StepThroughProceduralInstruction(modules, positiveTestInstr);
                ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

                BitAssert.Equality(one, modules.Registers.GetLoRegContent(Regs.Ass.GetIndex()));
            } else {
                BitAssert.Equality(skipAddress, modules.Memory.PcContent);
                StepThroughProceduralInstruction(modules, negativeTestInstr);
                ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

                BitAssert.Equality(five, modules.Registers.GetLoRegContent(Regs.Ass.GetIndex()));
            }
        }


        [Theory]
        [InlineData(-5, -5, 0xFF00)]
        [InlineData(15, -14, 0x233)]
        [InlineData(-15, 15, 0x2137)]
        [InlineData(0, 0, 0xFF00)]
        public void Jzf(sbyte valueAStr, sbyte valueBStr, ushort addrStr) {
            var preInstruction = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.Add));
            var instruction = McConditionalInstruction.CreateFromSteps(typeof(JumpConditionalInstructions), nameof(JumpConditionalInstructions.Jzf));
            var positiveTestInstr = McProceduralInstruction.CreateFromSteps(typeof(RegsProceduralInstructions), nameof(RegsProceduralInstructions.SetI));
            var negativeTestInstr = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.AddI));

            var valueA = BitArrayHelper.FromSByteLE(valueAStr);
            var valueB = BitArrayHelper.FromSByteLE(valueBStr);
            var addr = BitArrayHelper.FromUShortLE(addrStr);
            var one = BitArrayHelper.FromByteLE(231);
            var five = BitArrayHelper.FromByteLE(5);
            var skipAddress = BitArrayHelper.FromShortLE(4);
            var zero = BitArrayHelper.FromByteLE(0);

            EncodeInstruction(preInstruction, Regs.T1, Regs.S1, Regs.S2, out var preInstructionHigh, out var preInstructionLow);
            EncodeInstruction(instruction, Regs.Zero, Regs.Zero, Regs.T2, out var instructionHigh, out var instructionLow);
            EncodeInstruction(positiveTestInstr, Regs.Ass, one, out var positiveTestInstrHigh, out var positiveTestInstrLow);
            EncodeInstruction(negativeTestInstr, Regs.Ass, five, out var negativeTestInstrHigh, out var negativeTestInstrLow);

            var romData = new BitArray[0xFFFF + 1];
            romData[0] = preInstructionHigh;
            romData[1] = preInstructionLow;

            romData[2] = instructionHigh;
            romData[3] = instructionLow;

            romData[4] = negativeTestInstrHigh;
            romData[5] = negativeTestInstrLow;

            romData[addrStr] = positiveTestInstrHigh;
            romData[addrStr + 1] = positiveTestInstrLow;

            var cp = BuildPcModules(romData, out var modules);
            var lw = BuildLwEmulator(romData, null);

            modules.Registers.SetWholeRegContent(Regs.S1.GetIndex(), zero.MergeWith(valueA));
            modules.Registers.SetWholeRegContent(Regs.S2.GetIndex(), zero.MergeWith(valueB));
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);

            CopyRegsToLw(lw, modules);
            EmuLwIntegrity.AssertFullIntegrity(lw, modules);

            StepThroughProceduralInstruction(modules, preInstruction);
            ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);
            StepThroughConditionalInstruction(modules, instruction);
            ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));

            if (valueAStr + valueBStr == 0) {
                BitAssert.Equality(addr, modules.Memory.PcContent);
                StepThroughProceduralInstruction(modules, positiveTestInstr);
                ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

                BitAssert.Equality(one, modules.Registers.GetLoRegContent(Regs.Ass.GetIndex()));
            } else {
                BitAssert.Equality(skipAddress, modules.Memory.PcContent);
                StepThroughProceduralInstruction(modules, negativeTestInstr);
                ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

                BitAssert.Equality(five, modules.Registers.GetLoRegContent(Regs.Ass.GetIndex()));
            }
        }

        [Theory]
        [InlineData(-5, -5, 0xFF00)]
        [InlineData(15, -14, 0x233)]
        [InlineData(-15, 15, 0x2137)]
        [InlineData(0, 0, 0xFF00)]
        public void Jnf(sbyte valueAStr, sbyte valueBStr, ushort addrStr) {
            var preInstruction = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.Add));
            var instruction = McConditionalInstruction.CreateFromSteps(typeof(JumpConditionalInstructions), nameof(JumpConditionalInstructions.Jnf));
            var positiveTestInstr = McProceduralInstruction.CreateFromSteps(typeof(RegsProceduralInstructions), nameof(RegsProceduralInstructions.SetI));
            var negativeTestInstr = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.AddI));

            var valueA = BitArrayHelper.FromSByteLE(valueAStr);
            var valueB = BitArrayHelper.FromSByteLE(valueBStr);
            var addr = BitArrayHelper.FromUShortLE(addrStr);
            var one = BitArrayHelper.FromByteLE(231);
            var five = BitArrayHelper.FromByteLE(5);
            var skipAddress = BitArrayHelper.FromShortLE(4);
            var zero = BitArrayHelper.FromByteLE(0);

            EncodeInstruction(preInstruction, Regs.T1, Regs.S1, Regs.S2, out var preInstructionHigh, out var preInstructionLow);
            EncodeInstruction(instruction, Regs.Zero, Regs.Zero, Regs.T2, out var instructionHigh, out var instructionLow);
            EncodeInstruction(positiveTestInstr, Regs.Ass, one, out var positiveTestInstrHigh, out var positiveTestInstrLow);
            EncodeInstruction(negativeTestInstr, Regs.Ass, five, out var negativeTestInstrHigh, out var negativeTestInstrLow);

            var romData = new BitArray[0xFFFF + 1];
            romData[0] = preInstructionHigh;
            romData[1] = preInstructionLow;

            romData[2] = instructionHigh;
            romData[3] = instructionLow;

            romData[4] = negativeTestInstrHigh;
            romData[5] = negativeTestInstrLow;

            romData[addrStr] = positiveTestInstrHigh;
            romData[addrStr + 1] = positiveTestInstrLow;

            var cp = BuildPcModules(romData, out var modules);
            var lw = BuildLwEmulator(romData, null);

            modules.Registers.SetWholeRegContent(Regs.S1.GetIndex(), zero.MergeWith(valueA));
            modules.Registers.SetWholeRegContent(Regs.S2.GetIndex(), zero.MergeWith(valueB));
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);

            CopyRegsToLw(lw, modules);
            EmuLwIntegrity.AssertFullIntegrity(lw, modules);

            StepThroughProceduralInstruction(modules, preInstruction);
            ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);
            StepThroughConditionalInstruction(modules, instruction);
            ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));

            if (valueAStr + valueBStr < 0) {
                BitAssert.Equality(addr, modules.Memory.PcContent);
                StepThroughProceduralInstruction(modules, positiveTestInstr);
                ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

                BitAssert.Equality(one, modules.Registers.GetLoRegContent(Regs.Ass.GetIndex()));
            } else {
                BitAssert.Equality(skipAddress, modules.Memory.PcContent);
                StepThroughProceduralInstruction(modules, negativeTestInstr);
                ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

                BitAssert.Equality(five, modules.Registers.GetLoRegContent(Regs.Ass.GetIndex()));
            }
        }

        [Theory]
        [InlineData(255, 0, 0xFF00)]
        [InlineData(255, 1, 0x233)]
        [InlineData(150, 150, 0x2137)]
        [InlineData(0, 0, 0xFF00)]
        public void Jcf(byte valueAStr, byte valueBStr, ushort addrStr) {
            var preInstruction = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.Add));
            var instruction = McConditionalInstruction.CreateFromSteps(typeof(JumpConditionalInstructions), nameof(JumpConditionalInstructions.Jcf));
            var positiveTestInstr = McProceduralInstruction.CreateFromSteps(typeof(RegsProceduralInstructions), nameof(RegsProceduralInstructions.SetI));
            var negativeTestInstr = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.AddI));

            var valueA = BitArrayHelper.FromByteLE(valueAStr);
            var valueB = BitArrayHelper.FromByteLE(valueBStr);
            var addr = BitArrayHelper.FromUShortLE(addrStr);
            var one = BitArrayHelper.FromByteLE(231);
            var five = BitArrayHelper.FromByteLE(5);
            var skipAddress = BitArrayHelper.FromShortLE(4);
            var zero = BitArrayHelper.FromByteLE(0);

            EncodeInstruction(preInstruction, Regs.T1, Regs.S1, Regs.S2, out var preInstructionHigh, out var preInstructionLow);
            EncodeInstruction(instruction, Regs.Zero, Regs.Zero, Regs.T2, out var instructionHigh, out var instructionLow);
            EncodeInstruction(positiveTestInstr, Regs.Ass, one, out var positiveTestInstrHigh, out var positiveTestInstrLow);
            EncodeInstruction(negativeTestInstr, Regs.Ass, five, out var negativeTestInstrHigh, out var negativeTestInstrLow);

            var romData = new BitArray[0xFFFF + 1];
            romData[0] = preInstructionHigh;
            romData[1] = preInstructionLow;

            romData[2] = instructionHigh;
            romData[3] = instructionLow;

            romData[4] = negativeTestInstrHigh;
            romData[5] = negativeTestInstrLow;

            romData[addrStr] = positiveTestInstrHigh;
            romData[addrStr + 1] = positiveTestInstrLow;

            var cp = BuildPcModules(romData, out var modules);
            var lw = BuildLwEmulator(romData, null);

            modules.Registers.SetWholeRegContent(Regs.S1.GetIndex(), zero.MergeWith(valueA));
            modules.Registers.SetWholeRegContent(Regs.S2.GetIndex(), zero.MergeWith(valueB));
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);

            CopyRegsToLw(lw, modules);
            EmuLwIntegrity.AssertFullIntegrity(lw, modules);

            StepThroughProceduralInstruction(modules, preInstruction);
            ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);
            StepThroughConditionalInstruction(modules, instruction);
            ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));

            if (valueAStr + valueBStr > byte.MaxValue) {
                BitAssert.Equality(addr, modules.Memory.PcContent);
                StepThroughProceduralInstruction(modules, positiveTestInstr);
                ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

                BitAssert.Equality(one, modules.Registers.GetLoRegContent(Regs.Ass.GetIndex()));
            } else {
                BitAssert.Equality(skipAddress, modules.Memory.PcContent);
                StepThroughProceduralInstruction(modules, negativeTestInstr);
                ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

                BitAssert.Equality(five, modules.Registers.GetLoRegContent(Regs.Ass.GetIndex()));
            }
        }

        [Theory]
        [InlineData(-128, 0, 0xFF00)]
        [InlineData(-128, -1, 0x233)]
        [InlineData(127, 0, 0x2137)]
        [InlineData(127, 1, 0xFF00)]
        public void Jof(sbyte valueAStr, sbyte valueBStr, ushort addrStr) {
            var preInstruction = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.Add));
            var instruction = McConditionalInstruction.CreateFromSteps(typeof(JumpConditionalInstructions), nameof(JumpConditionalInstructions.Jof));
            var positiveTestInstr = McProceduralInstruction.CreateFromSteps(typeof(RegsProceduralInstructions), nameof(RegsProceduralInstructions.SetI));
            var negativeTestInstr = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.AddI));

            var valueA = BitArrayHelper.FromSByteLE(valueAStr);
            var valueB = BitArrayHelper.FromSByteLE(valueBStr);
            var addr = BitArrayHelper.FromUShortLE(addrStr);
            var one = BitArrayHelper.FromByteLE(231);
            var five = BitArrayHelper.FromByteLE(5);
            var skipAddress = BitArrayHelper.FromShortLE(4);
            var zero = BitArrayHelper.FromByteLE(0);

            EncodeInstruction(preInstruction, Regs.T1, Regs.S1, Regs.S2, out var preInstructionHigh, out var preInstructionLow);
            EncodeInstruction(instruction, Regs.Zero, Regs.Zero, Regs.T2, out var instructionHigh, out var instructionLow);
            EncodeInstruction(positiveTestInstr, Regs.Ass, one, out var positiveTestInstrHigh, out var positiveTestInstrLow);
            EncodeInstruction(negativeTestInstr, Regs.Ass, five, out var negativeTestInstrHigh, out var negativeTestInstrLow);

            var romData = new BitArray[0xFFFF + 1];
            romData[0] = preInstructionHigh;
            romData[1] = preInstructionLow;

            romData[2] = instructionHigh;
            romData[3] = instructionLow;

            romData[4] = negativeTestInstrHigh;
            romData[5] = negativeTestInstrLow;

            romData[addrStr] = positiveTestInstrHigh;
            romData[addrStr + 1] = positiveTestInstrLow;

            var cp = BuildPcModules(romData, out var modules);
            var lw = BuildLwEmulator(romData, null);

            modules.Registers.SetWholeRegContent(Regs.S1.GetIndex(), zero.MergeWith(valueA));
            modules.Registers.SetWholeRegContent(Regs.S2.GetIndex(), zero.MergeWith(valueB));
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);

            CopyRegsToLw(lw, modules);
            EmuLwIntegrity.AssertFullIntegrity(lw, modules);

            StepThroughProceduralInstruction(modules, preInstruction);
            ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);
            StepThroughConditionalInstruction(modules, instruction);
            ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));

            if (valueAStr + valueBStr > sbyte.MaxValue || valueAStr + valueBStr < sbyte.MinValue) {
                BitAssert.Equality(addr, modules.Memory.PcContent);
                StepThroughProceduralInstruction(modules, positiveTestInstr);
                ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

                BitAssert.Equality(one, modules.Registers.GetLoRegContent(Regs.Ass.GetIndex()));
            } else {
                BitAssert.Equality(skipAddress, modules.Memory.PcContent);
                StepThroughProceduralInstruction(modules, negativeTestInstr);
                ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

                BitAssert.Equality(five, modules.Registers.GetLoRegContent(Regs.Ass.GetIndex()));
            }
        }
    }
}

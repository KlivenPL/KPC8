using Infrastructure.BitArrays;
using KPC8.Microcode;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections;
using Tests._Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Tests.KPC8Tests.Microcode.Instructions {
    public class JumpProceduralInstructionTests : McInstructionTestBase {
        public JumpProceduralInstructionTests(ITestOutputHelper debug) : base(debug) {

        }

        [Theory]
        [InlineData(10)]
        [InlineData(0xFF00)]
        [InlineData(0x2137)]
        public void Jr(ushort addrStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(JumpProceduralInstructions), nameof(JumpProceduralInstructions.Jr));
            var addTestInstr = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.AddI));

            var addr = BitArrayHelper.FromUShortLE(addrStr);
            var one = BitArrayHelper.FromByteLE(1);

            EncodeInstruction(instruction, Regs.Zero, Regs.Zero, Regs.T1, out var instructionHigh, out var instructionLow);
            EncodeInstruction(addTestInstr, Regs.T4, one, out var addTestInstrHigh, out var addTestInstrLow);

            var romData = new BitArray[0xFFFF + 1];
            romData[0] = instructionHigh;
            romData[1] = instructionLow;
            romData[addrStr] = addTestInstrHigh;
            romData[addrStr + 1] = addTestInstrLow;

            var cp = BuildPcModules(romData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), addr);

            StepThroughInstruction(modules, instruction);
            BitAssert.Equality(addr, modules.Memory.PcContent);
            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));

            StepThroughInstruction(modules, addTestInstr);

            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(one, modules.Registers.GetLoRegContent(Regs.T4.GetIndex()));
        }

        [Theory]
        [InlineData(0x00FF, 0x00)]
        [InlineData(0x00FF, 0xEF)]
        [InlineData(0x21, 0x37)]
        public void Jro(ushort addrStr, ushort offsetStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(JumpProceduralInstructions), nameof(JumpProceduralInstructions.Jro));
            var addTestInstr = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.AddI));

            var addr = BitArrayHelper.FromUShortLE(addrStr);
            var offset = BitArrayHelper.FromUShortLE(offsetStr);
            var addrPlusOffset = BitArrayHelper.FromUShortLE((ushort)(addrStr + offsetStr));
            var one = BitArrayHelper.FromByteLE(1);

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh, out var instructionLow);
            EncodeInstruction(addTestInstr, Regs.T4, one, out var addTestInstrHigh, out var addTestInstrLow);

            var romData = new BitArray[0xFFFF + 1];
            romData[0] = instructionHigh;
            romData[1] = instructionLow;
            romData[addrStr + offsetStr] = addTestInstrHigh;
            romData[addrStr + offsetStr + 1] = addTestInstrLow;

            var cp = BuildPcModules(romData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), addr);
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), offset);

            StepThroughInstruction(modules, instruction);
            BitAssert.Equality(addrPlusOffset, modules.Memory.PcContent);
            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));

            StepThroughInstruction(modules, addTestInstr);

            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(offset, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));
            BitAssert.Equality(one, modules.Registers.GetLoRegContent(Regs.T4.GetIndex()));
        }

        [Theory]
        [InlineData(0x37)]
        [InlineData(0x00FF)]
        [InlineData(0xFF00)]
        public void Jas(ushort addrStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(JumpProceduralInstructions), nameof(JumpProceduralInstructions.Jas));
            var addTestInstr = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.AddI));

            var addr = BitArrayHelper.FromUShortLE(addrStr);
            var one = BitArrayHelper.FromByteLE(1);
            var savedPcAddr = BitArrayHelper.FromUShortLE(2);

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.Ra, out var instructionHigh, out var instructionLow);
            EncodeInstruction(addTestInstr, Regs.T4, one, out var addTestInstrHigh, out var addTestInstrLow);

            var romData = new BitArray[0xFFFF + 1];
            romData[0] = instructionHigh;
            romData[1] = instructionLow;
            romData[addrStr] = addTestInstrHigh;
            romData[addrStr + 1] = addTestInstrLow;

            var cp = BuildPcModules(romData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), addr);

            StepThroughInstruction(modules, instruction);
            BitAssert.Equality(addr, modules.Memory.PcContent);
            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));

            StepThroughInstruction(modules, addTestInstr);

            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(savedPcAddr, modules.Registers.GetWholeRegContent(Regs.Ra.GetIndex()));
            BitAssert.Equality(one, modules.Registers.GetLoRegContent(Regs.T4.GetIndex()));
        }

        [Theory]
        [InlineData(0x00)]
        [InlineData(0xFD)]
        [InlineData(0xFF)]
        public void JpcaddI(byte offsetStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(JumpProceduralInstructions), nameof(JumpProceduralInstructions.JpcaddI));
            var addTestInstr = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.AddI));

            var offset = BitArrayHelper.FromByteLE(offsetStr);
            var pcPlusOffset = BitArrayHelper.FromUShortLE((ushort)(2 + offsetStr));
            var one = BitArrayHelper.FromByteLE(1);

            EncodeInstruction(instruction, Regs.Zero, offset, out var instructionHigh, out var instructionLow);
            EncodeInstruction(addTestInstr, Regs.T4, one, out var addTestInstrHigh, out var addTestInstrLow);

            var romData = new BitArray[0xFFFF + 1];
            romData[0] = instructionHigh;
            romData[1] = instructionLow;
            romData[2 + offsetStr] = addTestInstrHigh;
            romData[2 + offsetStr + 1] = addTestInstrLow;

            var cp = BuildPcModules(romData, out var modules);

            StepThroughInstruction(modules, instruction);
            BitAssert.Equality(pcPlusOffset, modules.Memory.PcContent);

            StepThroughInstruction(modules, addTestInstr);

            BitAssert.Equality(one, modules.Registers.GetLoRegContent(Regs.T4.GetIndex()));
        }

        [Theory]
        [InlineData(0x21)]
        [InlineData(0xFD)]
        [InlineData(0xFF)]
        public void JpcsubI(byte offsetStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(JumpProceduralInstructions), nameof(JumpProceduralInstructions.JpcsubI));
            var primaryJumpInstr = McProceduralInstruction.CreateFromSteps(typeof(JumpProceduralInstructions), nameof(JumpProceduralInstructions.Jr));
            var addTestInstr = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.AddI));

            var offset = BitArrayHelper.FromByteLE(offsetStr);
            var pcPlusOffset = BitArrayHelper.FromUShortLE((ushort)(0xFFFF - offsetStr));
            var one = BitArrayHelper.FromByteLE(1);
            var primaryJumpAddr = BitArrayHelper.FromUShortLE(0xFFFD);

            EncodeInstruction(primaryJumpInstr, Regs.Zero, Regs.Zero, Regs.T1, out var primaryJumpInstrHigh, out var primaryJumpInstrLow);
            EncodeInstruction(instruction, Regs.Zero, offset, out var instructionHigh, out var instructionLow);
            EncodeInstruction(addTestInstr, Regs.T4, one, out var addTestInstrHigh, out var addTestInstrLow);

            var romData = new BitArray[0xFFFF + 1];
            romData[0] = primaryJumpInstrHigh;
            romData[1] = primaryJumpInstrLow;

            romData[0xFFFD] = instructionHigh;
            romData[0xFFFD + 1] = instructionLow;

            romData[0xFFFF - offsetStr] = addTestInstrHigh;
            romData[0xFFFF - offsetStr + 1] = addTestInstrLow;

            var cp = BuildPcModules(romData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), primaryJumpAddr);
            StepThroughInstruction(modules, primaryJumpInstr);

            BitAssert.Equality(primaryJumpAddr, modules.Memory.PcContent);

            StepThroughInstruction(modules, instruction);
            BitAssert.Equality(pcPlusOffset, modules.Memory.PcContent);

            StepThroughInstruction(modules, addTestInstr);

            BitAssert.Equality(one, modules.Registers.GetLoRegContent(Regs.T4.GetIndex()));
        }
    }
}

using Infrastructure.BitArrays;
using KPC8.Microcode;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using Tests._Infrastructure;
using Xunit;

namespace Tests.KPC8Tests.Microcode.Instructions {
    public class MathProceduralInstructionTests : McInstructionTestBase {
        [Fact]
        public void AddI_RunOnce_T2ContainsImmediateValue() {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.AddI));
            EncodeInstruction(instruction, Regs.T2, BitArrayHelper.FromString($"00101100"), out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow,
            };

            var cp = BuildPcModules(romData, out var modules);
            StepThroughInstruction(modules, instruction);

            BitAssert.Equality(instructionLow, modules.Registers.GetLoRegContent(Regs.T2.GetIndex()));
        }

        [Fact]
        public void AddI_RunTwice_T2ContainsSumResult() {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.AddI));
            var expectedSum = BitArrayHelper.FromString("00101111");

            EncodeInstruction(instruction, Regs.T2, BitArrayHelper.FromString($"00101100"), out var addIInstructionHigh1, out var addIInstructionLow1);
            EncodeInstruction(instruction, Regs.T2, BitArrayHelper.FromString($"00000011"), out var addIInstructionHigh2, out var addIInstructionLow2);

            var romData = new[] {
                addIInstructionHigh1, addIInstructionLow1,
                addIInstructionHigh2, addIInstructionLow2,
            };

            var cp = BuildPcModules(romData, out var modules);

            StepThroughInstruction(modules, instruction);
            StepThroughInstruction(modules, instruction);

            BitAssert.Equality(expectedSum, modules.Registers.GetLoRegContent(Regs.T2.GetIndex()));
        }

        [Fact]
        public void AddI_RunTwice_ThenAdd_T4ContainsSumResult() {
            var addIInstruction = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.AddI));
            var addInstruction = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.Add));
            var expectedSum = BitArrayHelper.FromString("01101111");

            EncodeInstruction(addIInstruction, Regs.T1, BitArrayHelper.FromString($"00101100"), out var addIInstructionHigh1, out var addIInstructionLow1);
            EncodeInstruction(addIInstruction, Regs.T2, BitArrayHelper.FromString($"01000011"), out var addIInstructionHigh2, out var addIInstructionLow2);
            EncodeInstruction(addInstruction, Regs.T4, Regs.T1, Regs.T2, out var addInstructionHigh3, out var addInstructionLow3);

            var romData = new[] {
                addIInstructionHigh1, addIInstructionLow1,
                addIInstructionHigh2, addIInstructionLow2,
                addInstructionHigh3, addInstructionLow3,
            };

            var cp = BuildPcModules(romData, out var modules);

            StepThroughInstruction(modules, addIInstruction);
            StepThroughInstruction(modules, addIInstruction);
            StepThroughInstruction(modules, addInstruction);

            BitAssert.Equality(addIInstructionLow1, modules.Registers.GetLoRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(addIInstructionLow2, modules.Registers.GetLoRegContent(Regs.T2.GetIndex()));
            BitAssert.Equality(expectedSum, modules.Registers.GetLoRegContent(Regs.T4.GetIndex()));
        }

        [Fact]
        public void SubI_RunOnce_T2ContainsInvertedImmediateValue() {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.SubI));
            var expectedResult = BitArrayHelper.FromString($"11010100");
            EncodeInstruction(instruction, Regs.T2, BitArrayHelper.FromString($"00101100"), out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow,
            };

            var cp = BuildPcModules(romData, out var modules);
            StepThroughInstruction(modules, instruction);

            BitAssert.Equality(expectedResult, modules.Registers.GetLoRegContent(Regs.T2.GetIndex()));
        }

        [Fact]
        public void SubI_RunTwice_T2ContainsSubResult() {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.SubI));
            var expectedResult = BitArrayHelper.FromString("11010001");

            EncodeInstruction(instruction, Regs.T2, BitArrayHelper.FromString($"00101100"), out var subIInstructionHigh1, out var subIInstructionLow1);
            EncodeInstruction(instruction, Regs.T2, BitArrayHelper.FromString($"00000011"), out var subIInstructionHigh2, out var subIInstructionLow2);

            var romData = new[] {
                subIInstructionHigh1, subIInstructionLow1,
                subIInstructionHigh2, subIInstructionLow2,
            };

            var cp = BuildPcModules(romData, out var modules);

            StepThroughInstruction(modules, instruction);
            StepThroughInstruction(modules, instruction);

            BitAssert.Equality(expectedResult, modules.Registers.GetLoRegContent(Regs.T2.GetIndex()));
        }

        [Fact]
        public void SubI_RunTwice_ThenAdd_T4ContainsSubResult() {
            var subIInstruction = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.SubI));
            var subInstruction = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.Sub));
            var expectedT1 = BitArrayHelper.FromString($"11010100");
            var expectedT2 = BitArrayHelper.FromString($"11111001");
            var expectedResult = BitArrayHelper.FromString("11011011");

            EncodeInstruction(subIInstruction, Regs.T1, BitArrayHelper.FromString($"00101100"), out var subIInstructionHigh1, out var subIInstructionLow1);
            EncodeInstruction(subIInstruction, Regs.T2, BitArrayHelper.FromString($"00000111"), out var subIInstructionHigh2, out var subIInstructionLow2);
            EncodeInstruction(subInstruction, Regs.T4, Regs.T1, Regs.T2, out var subInstructionHigh3, out var subInstructionLow3);

            var romData = new[] {
                subIInstructionHigh1, subIInstructionLow1,
                subIInstructionHigh2, subIInstructionLow2,
                subInstructionHigh3, subInstructionLow3,
            };

            var cp = BuildPcModules(romData, out var modules);

            StepThroughInstruction(modules, subIInstruction);
            StepThroughInstruction(modules, subIInstruction);
            StepThroughInstruction(modules, subInstruction);

            BitAssert.Equality(expectedT1, modules.Registers.GetLoRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(expectedT2, modules.Registers.GetLoRegContent(Regs.T2.GetIndex()));
            BitAssert.Equality(expectedResult, modules.Registers.GetLoRegContent(Regs.T4.GetIndex()));
        }
    }
}

using Infrastructure.BitArrays;
using KPC8.Microcode;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using Tests._Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Tests.KPC8Tests.Microcode.Instructions {
    public class MathProceduralInstructionTests : McInstructionTestBase {
        public MathProceduralInstructionTests(ITestOutputHelper debug) : base(debug) {

        }

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
        public void SubI_RunTwice_ThenSub_T4ContainsSubResult() {
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

        [Fact]
        public void Sub_SameNumbers_ResultZero() {
            var subInstruction = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.Sub));
            var zero = BitArrayHelper.FromString("00000000");
            var random = BitArrayHelper.FromString($"10101010");
            var expectedT1 = BitArrayHelper.FromString($"11010100");
            var expectedT2 = BitArrayHelper.FromString($"11010100");
            var expectedResult = BitArrayHelper.FromString("00000000");

            EncodeInstruction(subInstruction, Regs.T4, Regs.T1, Regs.T2, out var subInstructionHigh1, out var subInstructionLow1);

            var romData = new[] {
                subInstructionHigh1, subInstructionLow1,
            };

            var cp = BuildPcModules(romData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), zero.MergeWith(expectedT1));
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), zero.MergeWith(expectedT2));

            modules.Registers.SetWholeRegContent(Regs.T4.GetIndex(), zero.MergeWith(random));

            BitAssert.Equality(random, modules.Registers.GetLoRegContent(Regs.T4.GetIndex()));
            StepThroughInstruction(modules, subInstruction);

            BitAssert.Equality(expectedT1, modules.Registers.GetLoRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(expectedT2, modules.Registers.GetLoRegContent(Regs.T2.GetIndex()));
            BitAssert.Equality(expectedResult, modules.Registers.GetLoRegContent(Regs.T4.GetIndex()));
        }

        [Theory]
        [InlineData("11111111 00000000", "00000000 11111111", "11111111 11111111")]
        [InlineData("11111111 11111111", "00000000 00100100", "00000000 00100011")]
        [InlineData("11111111 11011100", "10000000 00000000", "01111111 11011100")]
        [InlineData("11111111 11011100", "01111111 11111111", "01111111 11011011")]
        public void AddW(string aStr, string bStr, string resultStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.AddW));
            var aVal = BitArrayHelper.FromString(aStr);
            var bVal = BitArrayHelper.FromString(bStr);
            var result = BitArrayHelper.FromString(resultStr);

            EncodeInstruction(instruction, Regs.T1, Regs.T2, Regs.T3, out var addCHigh, out var addCLow);

            var romData = new[] {
                addCHigh, addCLow,
            };

            var cp = BuildPcModules(romData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), aVal);
            modules.Registers.SetWholeRegContent(Regs.T3.GetIndex(), bVal);

            StepThroughInstruction(modules, instruction);
            BitAssert.Equality(result, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
        }

        [Theory]
        [InlineData("00000000 00000000", "00000000 00000000")]
        [InlineData("00000000 00000001", "11111111 11111111")]
        [InlineData("00000000 11111111", "11111111 00000001")]
        [InlineData("11111111 00000000", "00000001 00000000")]
        [InlineData("00001000 01011001", "11110111 10100111")]
        [InlineData("11111111 11011100", "00000000 00100100")]
        public void NegW(string originalStr, string negatedStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.NegW));
            var original = BitArrayHelper.FromString(originalStr);
            var negated = BitArrayHelper.FromString(negatedStr);

            EncodeInstruction(instruction, Regs.T1, Regs.T2, Regs.Zero, out var negCHigh1, out var negCLow1);
            EncodeInstruction(instruction, Regs.T1, Regs.T1, Regs.Zero, out var negCHigh2, out var negCLow2);

            var romData = new[] {
                negCHigh1, negCLow1,
                negCHigh2, negCLow2,
            };

            var cp = BuildPcModules(romData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), original);

            StepThroughInstruction(modules, instruction);
            BitAssert.Equality(negated, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));

            StepThroughInstruction(modules, instruction);
            BitAssert.Equality(original, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
        }
    }
}

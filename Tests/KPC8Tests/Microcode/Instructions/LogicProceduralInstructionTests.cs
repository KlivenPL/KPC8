using Infrastructure.BitArrays;
using KPC8.Microcode;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using Tests._Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Tests.KPC8Tests.Microcode.Instructions {
    public class LogicProceduralInstructionTests : McInstructionTestBase {
        public LogicProceduralInstructionTests(ITestOutputHelper debug) : base(debug) {

        }

        [Theory]
        [InlineData("10101100", "11001111", "10000100")]
        [InlineData("00000000", "11001111", "00110000")]
        [InlineData("00000000", "00000000", "11111111")]
        public void Not(string valAStr, string valBStr, string notAddExpectedResult) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(LogicProceduralInstructions), nameof(LogicProceduralInstructions.Not));

            var zero = BitArrayHelper.FromByteLE(0);
            var valA = BitArrayHelper.FromString(valAStr);
            var valB = BitArrayHelper.FromString(valBStr);
            var expectedResult = BitArrayHelper.FromString(notAddExpectedResult);

            EncodeInstruction(instruction, Regs.T1, Regs.T2, Regs.T3, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow,
            };

            var cp = BuildPcModules(romData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), zero.MergeWith(valA));
            modules.Registers.SetWholeRegContent(Regs.T3.GetIndex(), zero.MergeWith(valB));

            StepThroughInstruction(modules, instruction);

            BitAssert.Equality(expectedResult, modules.Registers.GetLoRegContent(Regs.T1.GetIndex()));
        }

        [Theory]
        [InlineData("00000000", "00000000", "00000000")]
        [InlineData("11111111", "00000000", "11111111")]
        [InlineData("10101100", "11001111", "11101111")]
        public void Or(string valAStr, string valBStr, string orExpectedResult) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(LogicProceduralInstructions), nameof(LogicProceduralInstructions.Or));

            var zero = BitArrayHelper.FromByteLE(0);
            var valA = BitArrayHelper.FromString(valAStr);
            var valB = BitArrayHelper.FromString(valBStr);
            var expectedResult = BitArrayHelper.FromString(orExpectedResult);

            EncodeInstruction(instruction, Regs.T1, Regs.T2, Regs.T3, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow,
            };

            var cp = BuildPcModules(romData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), zero.MergeWith(valA));
            modules.Registers.SetWholeRegContent(Regs.T3.GetIndex(), zero.MergeWith(valB));

            StepThroughInstruction(modules, instruction);

            BitAssert.Equality(expectedResult, modules.Registers.GetLoRegContent(Regs.T1.GetIndex()));
        }

        [Theory]
        [InlineData("00000000", "00000000", "00000000")]
        [InlineData("11111111", "00000000", "00000000")]
        [InlineData("10101100", "11001111", "10001100")]
        public void And(string valAStr, string valBStr, string andExpectedResult) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(LogicProceduralInstructions), nameof(LogicProceduralInstructions.And));

            var zero = BitArrayHelper.FromByteLE(0);
            var valA = BitArrayHelper.FromString(valAStr);
            var valB = BitArrayHelper.FromString(valBStr);
            var expectedResult = BitArrayHelper.FromString(andExpectedResult);

            EncodeInstruction(instruction, Regs.T1, Regs.T2, Regs.T3, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow,
            };

            var cp = BuildPcModules(romData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), zero.MergeWith(valA));
            modules.Registers.SetWholeRegContent(Regs.T3.GetIndex(), zero.MergeWith(valB));

            StepThroughInstruction(modules, instruction);

            BitAssert.Equality(expectedResult, modules.Registers.GetLoRegContent(Regs.T1.GetIndex()));
        }

        [Theory]
        [InlineData("00000000", "00000001", "00000010")]
        [InlineData("11111111", "00000000", "11111110")]
        [InlineData("10101100", "11001111", "11110110")]
        public void Sll(string valAStr, string valBStr, string andExpectedResult) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(LogicProceduralInstructions), nameof(LogicProceduralInstructions.Sll));

            var zero = BitArrayHelper.FromByteLE(0);
            var valA = BitArrayHelper.FromString(valAStr);
            var valB = BitArrayHelper.FromString(valBStr);
            var expectedResult = BitArrayHelper.FromString(andExpectedResult);

            EncodeInstruction(instruction, Regs.T1, Regs.T2, Regs.T3, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow,
            };

            var cp = BuildPcModules(romData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), zero.MergeWith(valA));
            modules.Registers.SetWholeRegContent(Regs.T3.GetIndex(), zero.MergeWith(valB));

            StepThroughInstruction(modules, instruction);

            BitAssert.Equality(expectedResult, modules.Registers.GetLoRegContent(Regs.T1.GetIndex()));
        }

        [Theory]
        [InlineData("10000000", "00000001", "01000000")]
        [InlineData("11111111", "00000000", "01111111")]
        [InlineData("10101100", "11001111", "00111101")]
        public void Srl(string valAStr, string valBStr, string andExpectedResult) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(LogicProceduralInstructions), nameof(LogicProceduralInstructions.Srl));

            var zero = BitArrayHelper.FromByteLE(0);
            var valA = BitArrayHelper.FromString(valAStr);
            var valB = BitArrayHelper.FromString(valBStr);
            var expectedResult = BitArrayHelper.FromString(andExpectedResult);

            EncodeInstruction(instruction, Regs.T1, Regs.T2, Regs.T3, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow,
            };

            var cp = BuildPcModules(romData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), zero.MergeWith(valA));
            modules.Registers.SetWholeRegContent(Regs.T3.GetIndex(), zero.MergeWith(valB));

            StepThroughInstruction(modules, instruction);

            BitAssert.Equality(expectedResult, modules.Registers.GetLoRegContent(Regs.T1.GetIndex()));
        }
    }
}

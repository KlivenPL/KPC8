using Infrastructure.BitArrays;
using KPC8.Microcode;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using Tests._Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Tests.KPC8Tests.Integration.Instructions {
    public class RegsProceduralInstructionTests : McInstructionTestBase {
        public RegsProceduralInstructionTests(ITestOutputHelper debug) : base(debug) {

        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(21, 37)]
        [InlineData(255, 255)]
        public void Set(byte valALoStr, byte valBLoStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(RegsProceduralInstructions), nameof(RegsProceduralInstructions.Set));

            var zero = BitArrayHelper.FromByteLE(0);
            var valALo = BitArrayHelper.FromByteLE(valALoStr);
            var valBLo = BitArrayHelper.FromByteLE(valBLoStr);

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow,
            };

            var cp = BuildPcModules(romData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), zero.MergeWith(valALo));
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), zero.MergeWith(valBLo));

            StepThroughProceduralInstruction(modules, instruction);

            BitAssert.Equality(valBLo, modules.Registers.GetLoRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(valBLo, modules.Registers.GetLoRegContent(Regs.T2.GetIndex()));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(21)]
        [InlineData(255)]
        public void SetI(byte valLoImmStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(RegsProceduralInstructions), nameof(RegsProceduralInstructions.SetI));

            var zero = BitArrayHelper.FromByteLE(0);
            var valLoImm = BitArrayHelper.FromByteLE(valLoImmStr);

            EncodeInstruction(instruction, Regs.T1, valLoImm, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow,
            };

            var cp = BuildPcModules(romData, out var modules);
            StepThroughProceduralInstruction(modules, instruction);

            BitAssert.Equality(valLoImm, modules.Registers.GetLoRegContent(Regs.T1.GetIndex()));
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(21, 37)]
        [InlineData(255, 255)]
        public void Seth(byte valALoStr, byte valBHiStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(RegsProceduralInstructions), nameof(RegsProceduralInstructions.Seth));

            var zero = BitArrayHelper.FromByteLE(0);
            var valAHi = BitArrayHelper.FromByteLE(valALoStr);
            var valBHi = BitArrayHelper.FromByteLE(valBHiStr);

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow,
            };

            var cp = BuildPcModules(romData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), valAHi.MergeWith(zero));
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), valBHi.MergeWith(zero));

            StepThroughProceduralInstruction(modules, instruction);

            BitAssert.Equality(valBHi, modules.Registers.GetHiRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(valBHi, modules.Registers.GetHiRegContent(Regs.T2.GetIndex()));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(21)]
        [InlineData(255)]
        public void SethI(byte valHiImmStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(RegsProceduralInstructions), nameof(RegsProceduralInstructions.SethI));

            var zero = BitArrayHelper.FromByteLE(0);
            var valHiImm = BitArrayHelper.FromByteLE(valHiImmStr);

            EncodeInstruction(instruction, Regs.T1, valHiImm, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow,
            };

            var cp = BuildPcModules(romData, out var modules);
            StepThroughProceduralInstruction(modules, instruction);

            BitAssert.Equality(valHiImm, modules.Registers.GetHiRegContent(Regs.T1.GetIndex()));
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, 1)]
        [InlineData(21, 37)]
        [InlineData(2137, -911)]
        [InlineData(30000, 10000)]
        [InlineData(-1, 455)]
        [InlineData(-1, -455)]
        public void Setw(short valAStr, short valBStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(RegsProceduralInstructions), nameof(RegsProceduralInstructions.Setw));

            var zero = BitArrayHelper.FromByteLE(0);
            var valA = BitArrayHelper.FromShortLE(valAStr);
            var valB = BitArrayHelper.FromShortLE(valBStr);

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow,
            };

            var cp = BuildPcModules(romData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), valA);
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), valB);

            StepThroughProceduralInstruction(modules, instruction);

            BitAssert.Equality(valB, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(valB, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));
        }

        [Theory]
        [InlineData(0x0000, 0x0000)]
        [InlineData(0x0000, 0x0001)]
        [InlineData(0x0021, 0x0037)]
        [InlineData(0x2137, 0x0911)]
        [InlineData(0xFF00, 0x00FF)]
        public void Setloh(ushort valAStr, ushort valBStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(RegsProceduralInstructions), nameof(RegsProceduralInstructions.Setloh));

            var zero = BitArrayHelper.FromByteLE(0);
            var valA = BitArrayHelper.FromUShortLE(valAStr);
            var valB = BitArrayHelper.FromUShortLE(valBStr);

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow,
            };

            var cp = BuildPcModules(romData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), valA);
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), valB);

            StepThroughProceduralInstruction(modules, instruction);

            BitAssert.Equality(valB.Slice(8, 8).MergeWith(valB.Take(8)), modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(valB, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(21, 37)]
        [InlineData(255, 255)]
        public void Swap(byte valALoStr, byte valBLoStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(RegsProceduralInstructions), nameof(RegsProceduralInstructions.Swap));

            var zero = BitArrayHelper.FromByteLE(0);
            var valALo = BitArrayHelper.FromByteLE(valALoStr);
            var valBLo = BitArrayHelper.FromByteLE(valBLoStr);

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow,
            };

            var cp = BuildPcModules(romData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), zero.MergeWith(valALo));
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), zero.MergeWith(valBLo));

            StepThroughProceduralInstruction(modules, instruction);

            BitAssert.Equality(valBLo, modules.Registers.GetLoRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(valALo, modules.Registers.GetLoRegContent(Regs.T2.GetIndex()));
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(21, 37)]
        [InlineData(255, 255)]
        public void Swaph(byte valALoStr, byte valBHiStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(RegsProceduralInstructions), nameof(RegsProceduralInstructions.Swaph));

            var zero = BitArrayHelper.FromByteLE(0);
            var valAHi = BitArrayHelper.FromByteLE(valALoStr);
            var valBHi = BitArrayHelper.FromByteLE(valBHiStr);

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow,
            };

            var cp = BuildPcModules(romData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), valAHi.MergeWith(zero));
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), valBHi.MergeWith(zero));

            StepThroughProceduralInstruction(modules, instruction);

            BitAssert.Equality(valBHi, modules.Registers.GetHiRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(valAHi, modules.Registers.GetHiRegContent(Regs.T2.GetIndex()));
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, 1)]
        [InlineData(21, 37)]
        [InlineData(2137, -911)]
        [InlineData(30000, 10000)]
        [InlineData(-1, 455)]
        [InlineData(-1, -455)]
        public void Swapw(short valAStr, short valBStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(RegsProceduralInstructions), nameof(RegsProceduralInstructions.Swapw));

            var zero = BitArrayHelper.FromByteLE(0);
            var valA = BitArrayHelper.FromShortLE(valAStr);
            var valB = BitArrayHelper.FromShortLE(valBStr);

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow,
            };

            var cp = BuildPcModules(romData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), valA);
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), valB);

            StepThroughProceduralInstruction(modules, instruction);

            BitAssert.Equality(valB, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(valA, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));
        }

        [Theory]
        [InlineData(0x0000, 0x0000)]
        [InlineData(0x0000, 0x0001)]
        [InlineData(0x0021, 0x0037)]
        [InlineData(0x2137, 0x0911)]
        [InlineData(0xFF00, 0x00FF)]
        [InlineData(10, 10)]
        // todo poprawić żeby działało w miejscu
        public void Swaploh(ushort valAStr, ushort valBStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(RegsProceduralInstructions), nameof(RegsProceduralInstructions.Swaploh));

            var valA = BitArrayHelper.FromUShortLE(valAStr);
            var valB = BitArrayHelper.FromUShortLE(valBStr);

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow,
            };

            var cp = BuildPcModules(romData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), valA);
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), valB);

            StepThroughProceduralInstruction(modules, instruction);

            BitAssert.Equality(valB.Slice(8, 8).MergeWith(valB.Take(8)), modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(valA.Slice(8, 8).MergeWith(valA.Take(8)), modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));
        }
    }
}

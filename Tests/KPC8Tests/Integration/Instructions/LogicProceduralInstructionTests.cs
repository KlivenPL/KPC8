using Infrastructure.BitArrays;
using KPC8.Microcode;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using Tests._Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Tests.KPC8Tests.Integration.Instructions {
    public class LogicProceduralInstructionTests : McInstructionTestBase {
        public LogicProceduralInstructionTests(ITestOutputHelper debug) : base(debug) {

        }

        [Theory]
        [InlineData(0, 0, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(255, 0, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(123, 45, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(235, 1, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(23, 45, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(0, 255, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(255, 255, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(23, 23, Regs.T1, Regs.T2, Regs.T3)]

        [InlineData(0, 0, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(255, 0, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(123, 45, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(235, 1, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(23, 45, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(0, 255, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(255, 255, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(23, 23, Regs.T1, Regs.T1, Regs.T2)]
        public void Not(byte valAByte, byte valBByte, Regs rDest, Regs rA, Regs rB) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(LogicProceduralInstructions), nameof(LogicProceduralInstructions.Not));

            var zero = BitArrayHelper.FromByteLE(0);
            var valA = BitArrayHelper.FromByteLE(valAByte);
            var valB = BitArrayHelper.FromByteLE(valBByte);
            ushort random = 22543;
            var randomBa = BitArrayHelper.FromUShortLE(random);
            var expectedResult = BitArrayHelper.FromByteLE((byte)~(valAByte + valBByte));

            EncodeInstruction(instruction, rDest, rA, rB, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow,
            };

            var cp = BuildPcModules(romData, out var modules);
            var lw = BuildLwEmulator(romData, null);

            modules.Registers.SetWholeRegContent(rDest.GetIndex(), randomBa);
            modules.Registers.SetWholeRegContent(rA.GetIndex(), randomBa.Take(8).MergeWith(valA));
            modules.Registers.SetWholeRegContent(rB.GetIndex(), randomBa.Take(8).MergeWith(valB));

            CopyRegsToLw(lw, modules);
            EmuLwIntegrity.AssertFullIntegrity(lw, modules);

            StepThroughProceduralInstruction(modules, instruction);
            ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

            BitAssert.Equality(expectedResult, modules.Registers.GetLoRegContent(rDest.GetIndex()));
        }

        [Theory]
        [InlineData(0, 0, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(255, 0, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(123, 45, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(235, 1, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(23, 45, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(0, 255, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(255, 255, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(23, 23, Regs.T1, Regs.T2, Regs.T3)]

        [InlineData(0, 0, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(255, 0, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(123, 45, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(235, 1, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(23, 45, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(0, 255, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(255, 255, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(23, 23, Regs.T1, Regs.T1, Regs.T2)]
        public void Or(byte valAByte, byte valBByte, Regs rDest, Regs rA, Regs rB) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(LogicProceduralInstructions), nameof(LogicProceduralInstructions.Or));

            var zero = BitArrayHelper.FromByteLE(0);
            var valA = BitArrayHelper.FromByteLE(valAByte);
            var valB = BitArrayHelper.FromByteLE(valBByte);
            ushort random = 22543;
            var randomBa = BitArrayHelper.FromUShortLE(random);
            var expectedResult = BitArrayHelper.FromByteLE((byte)(valAByte | valBByte));

            EncodeInstruction(instruction, rDest, rA, rB, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow,
            };

            var cp = BuildPcModules(romData, out var modules);
            var lw = BuildLwEmulator(romData, null);

            modules.Registers.SetWholeRegContent(rDest.GetIndex(), randomBa);
            modules.Registers.SetWholeRegContent(rA.GetIndex(), randomBa.Take(8).MergeWith(valA));
            modules.Registers.SetWholeRegContent(rB.GetIndex(), randomBa.Take(8).MergeWith(valB));

            CopyRegsToLw(lw, modules);
            EmuLwIntegrity.AssertFullIntegrity(lw, modules);

            StepThroughProceduralInstruction(modules, instruction);
            ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

            BitAssert.Equality(expectedResult, modules.Registers.GetLoRegContent(rDest.GetIndex()));
        }

        [Theory]
        [InlineData(0, 0, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(255, 0, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(123, 45, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(235, 1, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(23, 45, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(0, 255, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(255, 255, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(23, 23, Regs.T1, Regs.T2, Regs.T3)]

        [InlineData(0, 0, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(255, 0, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(123, 45, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(235, 1, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(23, 45, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(0, 255, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(255, 255, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(23, 23, Regs.T1, Regs.T1, Regs.T2)]
        public void And(byte valAByte, byte valBByte, Regs rDest, Regs rA, Regs rB) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(LogicProceduralInstructions), nameof(LogicProceduralInstructions.And));

            var zero = BitArrayHelper.FromByteLE(0);
            var valA = BitArrayHelper.FromByteLE(valAByte);
            var valB = BitArrayHelper.FromByteLE(valBByte);
            ushort random = 22543;
            var randomBa = BitArrayHelper.FromUShortLE(random);
            var expectedResult = BitArrayHelper.FromByteLE((byte)(valAByte & valBByte));

            EncodeInstruction(instruction, rDest, rA, rB, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow,
            };

            var cp = BuildPcModules(romData, out var modules);
            var lw = BuildLwEmulator(romData, null);

            modules.Registers.SetWholeRegContent(rDest.GetIndex(), randomBa);
            modules.Registers.SetWholeRegContent(rA.GetIndex(), randomBa.Take(8).MergeWith(valA));
            modules.Registers.SetWholeRegContent(rB.GetIndex(), randomBa.Take(8).MergeWith(valB));

            CopyRegsToLw(lw, modules);
            EmuLwIntegrity.AssertFullIntegrity(lw, modules);

            StepThroughProceduralInstruction(modules, instruction);
            ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

            BitAssert.Equality(expectedResult, modules.Registers.GetLoRegContent(rDest.GetIndex()));
        }

        [Theory]
        [InlineData(0, 0, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(128, 1, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(255, 0, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(123, 45, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(235, 1, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(23, 45, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(0, 255, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(255, 255, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(23, 23, Regs.T1, Regs.T2, Regs.T3)]

        [InlineData(0, 0, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(128, 1, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(255, 0, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(123, 45, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(235, 1, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(23, 45, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(0, 255, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(255, 255, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(23, 23, Regs.T1, Regs.T1, Regs.T2)]
        public void Sll(byte valAByte, byte valBByte, Regs rDest, Regs rA, Regs rB) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(LogicProceduralInstructions), nameof(LogicProceduralInstructions.Sll));

            var zero = BitArrayHelper.FromByteLE(0);
            var valA = BitArrayHelper.FromByteLE(valAByte);
            var valB = BitArrayHelper.FromByteLE(valBByte);
            ushort random = 22543;
            var randomBa = BitArrayHelper.FromUShortLE(random);
            var expectedResult = BitArrayHelper.FromByteLE((byte)((valAByte + valBByte) << 1));

            EncodeInstruction(instruction, rDest, rA, rB, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow,
            };

            var cp = BuildPcModules(romData, out var modules);
            var lw = BuildLwEmulator(romData, null);

            modules.Registers.SetWholeRegContent(rDest.GetIndex(), randomBa);
            modules.Registers.SetWholeRegContent(rA.GetIndex(), randomBa.Take(8).MergeWith(valA));
            modules.Registers.SetWholeRegContent(rB.GetIndex(), randomBa.Take(8).MergeWith(valB));

            CopyRegsToLw(lw, modules);

            EmuLwIntegrity.AssertFullIntegrity(lw, modules);

            StepThroughProceduralInstruction(modules, instruction);
            ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

            BitAssert.Equality(expectedResult, modules.Registers.GetLoRegContent(rDest.GetIndex()));
        }

        [Theory]
        [InlineData(0, 0, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(255, 0, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(123, 45, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(235, 1, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(23, 45, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(0, 255, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(255, 255, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(23, 23, Regs.T1, Regs.T2, Regs.T3)]

        [InlineData(0, 0, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(255, 0, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(123, 45, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(235, 1, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(23, 45, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(0, 255, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(255, 255, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(23, 23, Regs.T1, Regs.T1, Regs.T2)]
        public void Xor(byte valAByte, byte valBByte, Regs rDest, Regs rA, Regs rB) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(LogicProceduralInstructions), nameof(LogicProceduralInstructions.Xor));

            var zero = BitArrayHelper.FromByteLE(0);
            var valA = BitArrayHelper.FromByteLE(valAByte);
            var valB = BitArrayHelper.FromByteLE(valBByte);
            ushort random = 22543;
            var randomBa = BitArrayHelper.FromUShortLE(random);
            var expectedResult = BitArrayHelper.FromByteLE((byte)(valAByte ^ valBByte));

            EncodeInstruction(instruction, rDest, rA, rB, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow,
            };

            var cp = BuildPcModules(romData, out var modules);
            var lw = BuildLwEmulator(romData, null);

            modules.Registers.SetWholeRegContent(rDest.GetIndex(), randomBa);
            modules.Registers.SetWholeRegContent(rA.GetIndex(), randomBa.Take(8).MergeWith(valA));
            modules.Registers.SetWholeRegContent(rB.GetIndex(), randomBa.Take(8).MergeWith(valB));

            CopyRegsToLw(lw, modules);

            EmuLwIntegrity.AssertFullIntegrity(lw, modules);

            StepThroughProceduralInstruction(modules, instruction);
            ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

            BitAssert.Equality(expectedResult, modules.Registers.GetLoRegContent(rDest.GetIndex()));
        }

        [Theory]
        [InlineData(0, 0, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(128, 1, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(255, 0, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(123, 45, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(235, 1, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(23, 45, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(0, 255, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(255, 255, Regs.T1, Regs.T2, Regs.T3)]
        [InlineData(23, 23, Regs.T1, Regs.T2, Regs.T3)]

        [InlineData(0, 0, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(128, 1, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(255, 0, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(123, 45, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(235, 1, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(23, 45, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(0, 255, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(255, 255, Regs.T1, Regs.T1, Regs.T2)]
        [InlineData(23, 23, Regs.T1, Regs.T1, Regs.T2)]
        public void Srl(byte valAByte, byte valBByte, Regs rDest, Regs rA, Regs rB) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(LogicProceduralInstructions), nameof(LogicProceduralInstructions.Srl));

            var zero = BitArrayHelper.FromByteLE(0);
            var valA = BitArrayHelper.FromByteLE(valAByte);
            var valB = BitArrayHelper.FromByteLE(valBByte);
            ushort random = 22543;
            var randomBa = BitArrayHelper.FromUShortLE(random);
            var expectedResult = BitArrayHelper.FromByteLE((byte)(((uint)(byte)(valAByte + valBByte)) >> 1));

            EncodeInstruction(instruction, rDest, rA, rB, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow,
            };

            var cp = BuildPcModules(romData, out var modules);
            var lw = BuildLwEmulator(romData, null);

            modules.Registers.SetWholeRegContent(rDest.GetIndex(), randomBa);
            modules.Registers.SetWholeRegContent(rA.GetIndex(), randomBa.Take(8).MergeWith(valA));
            modules.Registers.SetWholeRegContent(rB.GetIndex(), randomBa.Take(8).MergeWith(valB));

            CopyRegsToLw(lw, modules);

            EmuLwIntegrity.AssertFullIntegrity(lw, modules);

            StepThroughProceduralInstruction(modules, instruction);
            ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

            BitAssert.Equality(expectedResult, modules.Registers.GetLoRegContent(rDest.GetIndex()));
        }
    }
}

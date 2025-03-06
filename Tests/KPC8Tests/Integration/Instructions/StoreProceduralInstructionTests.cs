using Infrastructure.BitArrays;
using KPC8.Microcode;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using Tests._Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Tests.KPC8Tests.Integration.Instructions {
    public class StoreProceduralInstructionTests : McInstructionTestBase {
        public StoreProceduralInstructionTests(ITestOutputHelper debug) : base(debug) {

        }

        [Theory]
        [InlineData(12, 0x00FF)]
        [InlineData(1, 0xFF00)]
        [InlineData(255, 0xFFFF)]
        public void Sbram(byte valStr, ushort addrStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(StoreProceduralInstructions), nameof(StoreProceduralInstructions.Sbram));

            var zero = BitArrayHelper.FromByteLE(0);
            var val = BitArrayHelper.FromByteLE(valStr);
            var addr = BitArrayHelper.FromUShortLE(addrStr);

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow
            };

            var cp = BuildPcModules(romData, out var modules);
            var lw = BuildLwEmulator(romData, null);

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), zero.MergeWith(val));
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);

            CopyRegsToLw(lw, modules);
            EmuLwIntegrity.AssertFullIntegrity(lw, modules);

            StepThroughProceduralInstruction(modules, instruction);
            ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

            BitAssert.Equality(val, modules.Memory.GetRamAt(addrStr));
            BitAssert.Equality(val, modules.Registers.GetLoRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));
        }

        [Theory]
        [InlineData(12, 0x00FF)]
        [InlineData(1, 0xFF00)]
        [InlineData(255, 0xFFFF)]
        public void SbramI(byte valStr, ushort addrStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(StoreProceduralInstructions), nameof(StoreProceduralInstructions.SbramI));

            var zero = BitArrayHelper.FromByteLE(0);
            var val = BitArrayHelper.FromByteLE(valStr);
            var addr = BitArrayHelper.FromUShortLE(addrStr);

            EncodeInstruction(instruction, Regs.T1, val, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow
            };

            var cp = BuildPcModules(romData, out var modules);
            var lw = BuildLwEmulator(romData, null);

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), addr);

            CopyRegsToLw(lw, modules);
            EmuLwIntegrity.AssertFullIntegrity(lw, modules);

            StepThroughProceduralInstruction(modules, instruction);
            ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

            BitAssert.Equality(val, modules.Memory.GetRamAt(addrStr));
            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
        }

        [Theory]
        [InlineData(11, 0x00FF, 0x00)]
        [InlineData(24, 0x00FF, 0xEF)]
        [InlineData(1, 0xFF00, 0x00FF)]
        public void Sbramo(byte valStr, ushort addrStr, ushort offsetStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(StoreProceduralInstructions), nameof(StoreProceduralInstructions.Sbramo));

            var zero = BitArrayHelper.FromByteLE(0);
            var val = BitArrayHelper.FromByteLE(valStr);
            var addr = BitArrayHelper.FromUShortLE(addrStr);
            var offset = BitArrayHelper.FromUShortLE(offsetStr);

            EncodeInstruction(instruction, Regs.T1, Regs.T2, Regs.T3, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow
            };

            var cp = BuildPcModules(romData, out var modules);
            var lw = BuildLwEmulator(romData, null);

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), zero.MergeWith(val));
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);
            modules.Registers.SetWholeRegContent(Regs.T3.GetIndex(), offset);

            CopyRegsToLw(lw, modules);
            EmuLwIntegrity.AssertFullIntegrity(lw, modules);

            StepThroughProceduralInstruction(modules, instruction);
            ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

            BitAssert.Equality(val, modules.Memory.GetRamAt((ushort)(addrStr + offsetStr)));
            BitAssert.Equality(val, modules.Registers.GetLoRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));
            BitAssert.Equality(offset, modules.Registers.GetWholeRegContent(Regs.T3.GetIndex()));
        }

        [Theory]
        [InlineData(12, 0x00FF)]
        [InlineData(1, 0xFF00)]
        [InlineData(255, 0xFFFD)]
        [InlineData(2137, 0xFFFE)]
        [InlineData(16001, 0xFF)]
        [InlineData(21, 0x37)]
        public void Swram(ushort valStr, ushort addrStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(StoreProceduralInstructions), nameof(StoreProceduralInstructions.Swram));

            var val = BitArrayHelper.FromUShortLE(valStr);
            var addr = BitArrayHelper.FromUShortLE(addrStr);

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow
            };

            var cp = BuildPcModules(romData, out var modules);
            var lw = BuildLwEmulator(romData, null);

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), val);
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);

            CopyRegsToLw(lw, modules);
            EmuLwIntegrity.AssertFullIntegrity(lw, modules);

            StepThroughProceduralInstruction(modules, instruction);
            ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

            BitAssert.Equality(val.Take(8), modules.Memory.GetRamAt(addrStr));
            BitAssert.Equality(val.Skip(8), modules.Memory.GetRamAt((ushort)(addrStr + 1)));
            BitAssert.Equality(val, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));
        }

        [Theory]
        [InlineData(11, 0x00FF, 0x00)]
        [InlineData(24, 0x00FF, 0xEF)]
        [InlineData(11010, 0xFF00, 0x00FE)]
        [InlineData(2137, 0xFFF0, 0x1)]
        [InlineData(16001, 0xFF, 0x24)]
        public void Swramo(ushort valStr, ushort addrStr, ushort offsetStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(StoreProceduralInstructions), nameof(StoreProceduralInstructions.Swramo));

            var val = BitArrayHelper.FromUShortLE(valStr);
            var addr = BitArrayHelper.FromUShortLE(addrStr);
            var offset = BitArrayHelper.FromUShortLE(offsetStr);

            EncodeInstruction(instruction, Regs.T1, Regs.T2, Regs.T3, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow
            };

            var cp = BuildPcModules(romData, out var modules);
            var lw = BuildLwEmulator(romData, null);

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), val);
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);
            modules.Registers.SetWholeRegContent(Regs.T3.GetIndex(), offset);

            CopyRegsToLw(lw, modules);
            EmuLwIntegrity.AssertFullIntegrity(lw, modules);

            StepThroughProceduralInstruction(modules, instruction);
            ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

            BitAssert.Equality(val.Take(8), modules.Memory.GetRamAt((ushort)(addrStr + offsetStr)));
            BitAssert.Equality(val.Skip(8), modules.Memory.GetRamAt((ushort)(addrStr + offsetStr + 1)));
            BitAssert.Equality(val, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));
            BitAssert.Equality(offset, modules.Registers.GetWholeRegContent(Regs.T3.GetIndex()));
        }

        [Theory]
        [InlineData(3, 0)]
        [InlineData(12, 0x00FF)]
        [InlineData(1, 0xFF00)]
        [InlineData(255, 0xFFFF)]
        public void Pushb(byte valStr, ushort addrStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(StoreProceduralInstructions), nameof(StoreProceduralInstructions.Pushb));

            var zero = BitArrayHelper.FromByteLE(0);
            var val = BitArrayHelper.FromByteLE(valStr);
            var addr = BitArrayHelper.FromUShortLE(addrStr);
            var addrPlusOne = BitArrayHelper.FromUShortLE((ushort)(addrStr + 1));

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.Sp, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow
            };

            var cp = BuildPcModules(romData, out var modules);
            var lw = BuildLwEmulator(romData, null);

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), zero.MergeWith(val));
            modules.Registers.SetWholeRegContent(Regs.Sp.GetIndex(), addr);

            CopyRegsToLw(lw, modules);
            EmuLwIntegrity.AssertFullIntegrity(lw, modules);

            StepThroughProceduralInstruction(modules, instruction);
            ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

            BitAssert.Equality(val, modules.Memory.GetRamAt(addrStr));
            BitAssert.Equality(val, modules.Registers.GetLoRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(addrPlusOne, modules.Registers.GetWholeRegContent(Regs.Sp.GetIndex()));
        }

        [Theory]
        [InlineData(12, 0x00FF)]
        [InlineData(1, 0xFF00)]
        [InlineData(255, 0xFFFD)]
        [InlineData(2137, 0x453)]
        [InlineData(2137, 0xFFFE)]
        [InlineData(16001, 0xFF)]
        [InlineData(21, 0x37)]
        [InlineData(21, 257)]
        public void Pushw(ushort valStr, ushort addrStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(StoreProceduralInstructions), nameof(StoreProceduralInstructions.Pushw));

            var val = BitArrayHelper.FromUShortLE(valStr);
            var addr = BitArrayHelper.FromUShortLE(addrStr);
            var addrPlusTwo = BitArrayHelper.FromUShortLE((ushort)(addrStr + 2));

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow
            };

            var cp = BuildPcModules(romData, out var modules);
            var lw = BuildLwEmulator(romData, null);

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), val);
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);

            CopyRegsToLw(lw, modules);
            EmuLwIntegrity.AssertFullIntegrity(lw, modules);

            StepThroughProceduralInstruction(modules, instruction);
            ExecuteNextLwAndAssertIntegrityWithEmu(lw, modules);

            BitAssert.Equality(val.Take(8), modules.Memory.GetRamAt(addrStr));
            BitAssert.Equality(val.Skip(8), modules.Memory.GetRamAt((ushort)(addrStr + 1)));
            BitAssert.Equality(val, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(addrPlusTwo, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));
        }
    }
}

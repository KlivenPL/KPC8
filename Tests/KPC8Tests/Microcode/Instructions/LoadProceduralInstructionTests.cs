using Infrastructure.BitArrays;
using KPC8.Microcode;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections;
using Tests._Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Tests.KPC8Tests.Microcode.Instructions {
    public class LoadProceduralInstructionTests : McInstructionTestBase {
        public LoadProceduralInstructionTests(ITestOutputHelper debug) : base(debug) {

        }

        [Theory]
        [InlineData(12, 0x00FF)]
        [InlineData(1, 0xFF00)]
        [InlineData(255, 0xFFFF)]
        public void Lbrom(byte valStr, ushort addrStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(LoadProceduralInstructions), nameof(LoadProceduralInstructions.Lbrom));

            var val = BitArrayHelper.FromByteLE(valStr);
            var addr = BitArrayHelper.FromUShortLE(addrStr);

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh, out var instructionLow);

            var romData = new BitArray[0xFFFF + 1];
            romData[0] = instructionHigh;
            romData[1] = instructionLow;
            romData[addrStr] = val;

            var cp = BuildPcModules(romData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);

            StepThroughProceduralInstruction(modules, instruction);

            BitAssert.Equality(val, modules.Registers.GetLoRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));
        }

        [Theory]
        [InlineData(11, 0x00FF, 0x00)]
        [InlineData(24, 0x00FF, 0xEF)]
        [InlineData(1, 0xFF00, 0x00FF)]
        public void Lbromo(byte valStr, ushort addrStr, ushort offsetStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(LoadProceduralInstructions), nameof(LoadProceduralInstructions.Lbromo));

            var val = BitArrayHelper.FromByteLE(valStr);
            var addr = BitArrayHelper.FromUShortLE(addrStr);
            var offset = BitArrayHelper.FromUShortLE(offsetStr);

            EncodeInstruction(instruction, Regs.T1, Regs.T2, Regs.T3, out var instructionHigh, out var instructionLow);

            var romData = new BitArray[0xFFFF + 1];
            romData[0] = instructionHigh;
            romData[1] = instructionLow;
            romData[addrStr + offsetStr] = val;

            var cp = BuildPcModules(romData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);
            modules.Registers.SetWholeRegContent(Regs.T3.GetIndex(), offset);

            StepThroughProceduralInstruction(modules, instruction);

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
        public void Lwrom(ushort valStr, ushort addrStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(LoadProceduralInstructions), nameof(LoadProceduralInstructions.Lwrom));

            var val = BitArrayHelper.FromUShortLE(valStr);
            var addr = BitArrayHelper.FromUShortLE(addrStr);

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh, out var instructionLow);

            var romData = new BitArray[0xFFFF + 1];
            romData[0] = instructionHigh;
            romData[1] = instructionLow;
            romData[addrStr] = val.Take(8);
            romData[addrStr + 1] = val.Skip(8);

            var cp = BuildPcModules(romData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);

            StepThroughProceduralInstruction(modules, instruction);

            BitAssert.Equality(val, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));
        }

        [Theory]
        [InlineData(11, 0x00FF, 0x00)]
        [InlineData(24, 0x00FF, 0xEF)]
        [InlineData(11010, 0xFF00, 0x00FE)]
        [InlineData(2137, 0xFFF0, 0x1)]
        [InlineData(16001, 0xFF, 0x24)]
        public void Lwromo(ushort valStr, ushort addrStr, ushort offsetStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(LoadProceduralInstructions), nameof(LoadProceduralInstructions.Lwromo));

            var val = BitArrayHelper.FromUShortLE(valStr);
            var addr = BitArrayHelper.FromUShortLE(addrStr);
            var offset = BitArrayHelper.FromUShortLE(offsetStr);

            EncodeInstruction(instruction, Regs.T1, Regs.T2, Regs.T3, out var instructionHigh, out var instructionLow);

            var romData = new BitArray[0xFFFF + 1];
            romData[0] = instructionHigh;
            romData[1] = instructionLow;
            romData[addrStr + offsetStr] = val.Take(8);
            romData[addrStr + offsetStr + 1] = val.Skip(8);

            var cp = BuildPcModules(romData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);
            modules.Registers.SetWholeRegContent(Regs.T3.GetIndex(), offset);

            StepThroughProceduralInstruction(modules, instruction);

            BitAssert.Equality(val, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));
            BitAssert.Equality(offset, modules.Registers.GetWholeRegContent(Regs.T3.GetIndex()));
        }

        [Theory]
        [InlineData(12, 0x00FF)]
        [InlineData(1, 0xFF00)]
        [InlineData(255, 0xFFFF)]
        public void Lbram(byte valStr, ushort addrStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(LoadProceduralInstructions), nameof(LoadProceduralInstructions.Lbram));

            var val = BitArrayHelper.FromByteLE(valStr);
            var addr = BitArrayHelper.FromUShortLE(addrStr);

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow
            };

            var ramData = new BitArray[0xFFFF + 1];
            ramData[addrStr] = val;

            var cp = BuildPcModules(romData, ramData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);

            StepThroughProceduralInstruction(modules, instruction);

            BitAssert.Equality(val, modules.Registers.GetLoRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));
        }

        [Theory]
        [InlineData(11, 0x00FF, 0x00)]
        [InlineData(24, 0x00FF, 0xEF)]
        [InlineData(1, 0xFF00, 0x00FF)]
        public void Lbramo(byte valStr, ushort addrStr, ushort offsetStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(LoadProceduralInstructions), nameof(LoadProceduralInstructions.Lbramo));

            var val = BitArrayHelper.FromByteLE(valStr);
            var addr = BitArrayHelper.FromUShortLE(addrStr);
            var offset = BitArrayHelper.FromUShortLE(offsetStr);

            EncodeInstruction(instruction, Regs.T1, Regs.T2, Regs.T3, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow
            };

            var ramData = new BitArray[0xFFFF + 1];
            ramData[addrStr + offsetStr] = val;

            var cp = BuildPcModules(romData, ramData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);
            modules.Registers.SetWholeRegContent(Regs.T3.GetIndex(), offset);

            StepThroughProceduralInstruction(modules, instruction);

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
        public void Lwram(ushort valStr, ushort addrStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(LoadProceduralInstructions), nameof(LoadProceduralInstructions.Lwram));

            var val = BitArrayHelper.FromUShortLE(valStr);
            var addr = BitArrayHelper.FromUShortLE(addrStr);

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow
            };

            var ramData = new BitArray[0xFFFF + 1];
            ramData[addrStr] = val.Take(8);
            ramData[addrStr + 1] = val.Skip(8);

            var cp = BuildPcModules(romData, ramData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);

            StepThroughProceduralInstruction(modules, instruction);

            BitAssert.Equality(val, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));
        }

        [Theory]
        [InlineData(11, 0x00FF, 0x00)]
        [InlineData(24, 0x00FF, 0xEF)]
        [InlineData(11010, 0xFF00, 0x00FE)]
        [InlineData(2137, 0xFFF0, 0x1)]
        [InlineData(16001, 0xFF, 0x24)]
        public void Lwramo(ushort valStr, ushort addrStr, ushort offsetStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(LoadProceduralInstructions), nameof(LoadProceduralInstructions.Lwramo));

            var val = BitArrayHelper.FromUShortLE(valStr);
            var addr = BitArrayHelper.FromUShortLE(addrStr);
            var offset = BitArrayHelper.FromUShortLE(offsetStr);

            EncodeInstruction(instruction, Regs.T1, Regs.T2, Regs.T3, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow
            };

            var ramData = new BitArray[0xFFFF + 1];
            ramData[addrStr + offsetStr] = val.Take(8);
            ramData[addrStr + offsetStr + 1] = val.Skip(8);

            var cp = BuildPcModules(romData, ramData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);
            modules.Registers.SetWholeRegContent(Regs.T3.GetIndex(), offset);

            StepThroughProceduralInstruction(modules, instruction);

            BitAssert.Equality(val, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(addr, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));
            BitAssert.Equality(offset, modules.Registers.GetWholeRegContent(Regs.T3.GetIndex()));
        }

        [Theory]
        [InlineData(12, 0x00FF)]
        [InlineData(1, 0xFF00)]
        [InlineData(255, 0xFFFF)]
        public void Popb(byte valStr, ushort addrStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(LoadProceduralInstructions), nameof(LoadProceduralInstructions.Popb));

            var val = BitArrayHelper.FromByteLE(valStr);
            var addr = BitArrayHelper.FromUShortLE(addrStr);
            var addrMinusOne = BitArrayHelper.FromUShortLE((ushort)(addrStr - 1));

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow
            };

            var ramData = new BitArray[0xFFFF + 1];
            ramData[addrStr] = val;

            var cp = BuildPcModules(romData, ramData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);

            StepThroughProceduralInstruction(modules, instruction);

            BitAssert.Equality(val, modules.Registers.GetLoRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(addrMinusOne, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));
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
        public void Popw(ushort valStr, ushort addrStr) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(LoadProceduralInstructions), nameof(LoadProceduralInstructions.Popw));

            var val = BitArrayHelper.FromUShortLE(valStr);
            var addr = BitArrayHelper.FromUShortLE(addrStr);
            var addrMinusTwo = BitArrayHelper.FromUShortLE((ushort)(addrStr - 2));

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh, out var instructionLow);

            var romData = new[] {
                instructionHigh, instructionLow
            };

            var ramData = new BitArray[0xFFFF + 1];
            ramData[addrStr] = val.Take(8);
            ramData[addrStr + 1] = val.Skip(8);

            var cp = BuildPcModules(romData, ramData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), addr);

            StepThroughProceduralInstruction(modules, instruction);

            BitAssert.Equality(val, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(addrMinusTwo, modules.Registers.GetWholeRegContent(Regs.T2.GetIndex()));
        }
    }
}

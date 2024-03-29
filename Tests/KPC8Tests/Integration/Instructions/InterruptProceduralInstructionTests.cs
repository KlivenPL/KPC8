﻿using Components._Infrastructure.Components;
using Infrastructure.BitArrays;
using KPC8.Microcode;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections;
using Tests._Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Tests.KPC8Tests.Integration.Instructions {
    public class InterruptProceduralInstructionTests : McInstructionTestBase {
        public InterruptProceduralInstructionTests(ITestOutputHelper debug) : base(debug) {

        }

        [Fact]
        public void IrrEn() {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(InterruptProceduralInstructions), nameof(InterruptProceduralInstructions.Irren));

            EncodeInstruction(instruction, Regs.Zero, new BitArray(8), out var instructionHigh, out var instructionLow);

            var romData = new BitArray[] { instructionHigh, instructionLow };
            var cp = BuildPcModules(romData, out var modules);

            StepThroughProceduralInstruction(modules, instruction);
            Assert.True(modules.InterruptsBus.Lanes[2]);
        }

        [Fact]
        public void IrrDis() {
            var instructionEn = McProceduralInstruction.CreateFromSteps(typeof(InterruptProceduralInstructions), nameof(InterruptProceduralInstructions.Irren));
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(InterruptProceduralInstructions), nameof(InterruptProceduralInstructions.Irrdis));

            EncodeInstruction(instructionEn, Regs.Zero, new BitArray(8), out var instructionEnHigh, out var instructionEnLow);
            EncodeInstruction(instruction, Regs.Zero, new BitArray(8), out var instructionHigh, out var instructionLow);

            var romData = new BitArray[] { instructionEnHigh, instructionEnLow, instructionHigh, instructionLow };
            var cp = BuildPcModules(romData, out var modules);

            StepThroughProceduralInstruction(modules, instructionEn);
            Assert.True(modules.InterruptsBus.Lanes[2]);

            StepThroughProceduralInstruction(modules, instruction);
            Assert.False(modules.InterruptsBus.Lanes[2]);
        }

        [Theory]
        [InlineData(0xF0)]
        [InlineData(0x80)]
        [InlineData(0x00)]
        public void Irrex(byte addrLoStr) {
            var nop = McProceduralInstruction.CreateFromSteps(typeof(NopInstruction), nameof(NopInstruction.Nop));
            var irrex = McProceduralInstruction.CreateFromSteps(typeof(InterruptProceduralInstructions), nameof(InterruptProceduralInstructions.Irrex));
            var irren = McProceduralInstruction.CreateFromSteps(typeof(InterruptProceduralInstructions), nameof(InterruptProceduralInstructions.Irren));

            ushort fullAdrStr = (ushort)(0xFF00 + addrLoStr);

            var addrLo = BitArrayHelper.FromByteLE(addrLoStr);
            var zero = BitArrayHelper.FromByteLE(0);
            var testVal = BitArrayHelper.FromShortLE(2137);
            var addrFive = BitArrayHelper.FromShortLE(5);
            var fullAdr = BitArrayHelper.FromUShortLE(fullAdrStr);
            var flagsValue = BitArrayHelper.FromByteLE(0);

            EncodeInstruction(nop, Regs.Zero, Regs.Zero, Regs.Zero, out var nopHigh, out var nopLow);
            EncodeInstruction(irren, Regs.Zero, zero, out var irrenHigh, out var irrenLow);
            //EncodeInstruction(irrex, Regs.T1, addrLo, out var irrexHigh, out var irrexLow);

            var romData = new BitArray[0xFFFF + 1];

            romData[0] = irrenHigh;
            romData[1] = irrenLow;
            romData[2] = nopHigh;
            romData[3] = nopLow;
            //romData[2] = irrexHigh;
            //romData[3] = irrexLow;

            var cp = BuildPcModules(romData, out var modules);

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), testVal);

            // Prepare interrupt
            byte irrCode = (byte)(15 - addrLoStr / 16);
            modules.InterruptsBus.Write(BitArrayHelper.FromByteLE((byte)(0b10000000 | irrCode)));

            StepThroughProceduralInstruction(modules, irren);

            StepThroughProceduralInstruction(modules, nop);

            modules.Alu.SetRegFlagsContent(flagsValue.Skip(4));
            // Irrex is not stored in ROM - it is initiated by interrupt bus
            StepThroughProceduralInstruction(modules, irrex);

            BitAssert.Equality(flagsValue, modules.Memory.GetRamAt(0xFF00));

            BitAssert.Equality(addrFive.Take(8), modules.Memory.GetRamAt(0xFF02));
            BitAssert.Equality(addrFive.Skip(8), modules.Memory.GetRamAt(0xFF01));

            BitAssert.Equality(testVal.Take(8), modules.Memory.GetRamAt(0xFF03));
            BitAssert.Equality(testVal.Skip(8), modules.Memory.GetRamAt(0xFF04));


            BitAssert.Equality(fullAdr, modules.Memory.PcContent);
        }

        [Theory]
        [InlineData(0xF0)]
        [InlineData(0x80)]
        [InlineData(0x00)]
        public void Irrret(byte addrLoStr) {
            var nop = McProceduralInstruction.CreateFromSteps(typeof(NopInstruction), nameof(NopInstruction.Nop));
            var irrex = McProceduralInstruction.CreateFromSteps(typeof(InterruptProceduralInstructions), nameof(InterruptProceduralInstructions.Irrex));
            var irrret = McProceduralInstruction.CreateFromSteps(typeof(InterruptProceduralInstructions), nameof(InterruptProceduralInstructions.Irrret));
            var irren = McProceduralInstruction.CreateFromSteps(typeof(InterruptProceduralInstructions), nameof(InterruptProceduralInstructions.Irren));

            ushort fullAdrStr = (ushort)(0xFF00 + addrLoStr);

            var addrLo = BitArrayHelper.FromByteLE(addrLoStr);
            var zero8 = BitArrayHelper.FromByteLE(0);
            var zero16 = BitArrayHelper.FromUShortLE(0);
            var testVal = BitArrayHelper.FromShortLE(2137);
            var addrFour = BitArrayHelper.FromShortLE(4);
            var fullAdr = BitArrayHelper.FromUShortLE(fullAdrStr);
            var flagsValue = BitArrayHelper.FromByteLE(14);

            EncodeInstruction(nop, Regs.Zero, Regs.Zero, Regs.Zero, out var nopHigh, out var nopLow);
            // EncodeInstruction(irrex, Regs.T1, addrLo, out var irrexHigh, out var irrexLow);
            EncodeInstruction(irrret, Regs.T1, zero8, out var irrretHigh, out var irrretLow);
            EncodeInstruction(irren, Regs.Zero, zero8, out var irrenHigh, out var irrenLow);

            var romData = new BitArray[0xFFFF + 1];
            romData[0] = irrenHigh;
            romData[1] = irrenLow;
            romData[2] = nopHigh;
            romData[3] = nopLow;

            romData[fullAdrStr] = irrretHigh;
            romData[fullAdrStr + 1] = irrretLow;

            var cp = BuildPcModules(romData, out var modules);

            // Prepare interrupt
            byte irrCode = (byte)(15 - addrLoStr / 16);
            modules.InterruptsBus.Write(BitArrayHelper.FromByteLE((byte)(0b10000000 | irrCode)));

            StepThroughProceduralInstruction(modules, irren);

            StepThroughProceduralInstruction(modules, nop);

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), testVal);
            modules.Alu.SetRegFlagsContent(flagsValue.Skip(4));
            StepThroughProceduralInstruction(modules, irrex);

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), zero16);
            StepThroughProceduralInstruction(modules, irrret);

            BitAssert.Equality(testVal, modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(addrFour, modules.Memory.PcContent);
            BitAssert.Equality(flagsValue.Skip(4), modules.Alu.RegFlagsContent);
        }

        [Theory]
        [InlineData(0xF0)]
        [InlineData(0x80)]
        [InlineData(0x00)]
        public void InterruptFlow(byte addrLoStr) {
            var irrex = McProceduralInstruction.CreateFromSteps(typeof(InterruptProceduralInstructions), nameof(InterruptProceduralInstructions.Irrex));
            var irrret = McProceduralInstruction.CreateFromSteps(typeof(InterruptProceduralInstructions), nameof(InterruptProceduralInstructions.Irrret));
            var irren = McProceduralInstruction.CreateFromSteps(typeof(InterruptProceduralInstructions), nameof(InterruptProceduralInstructions.Irren));
            var add = McProceduralInstruction.CreateFromSteps(typeof(MathProceduralInstructions), nameof(MathProceduralInstructions.AddI));
            ushort fullAdrStr = (ushort)(0xFF00 + addrLoStr);

            var addrLo = BitArrayHelper.FromByteLE(addrLoStr);
            var zero = BitArrayHelper.FromByteLE(0);
            var three = BitArrayHelper.FromByteLE(3);
            var five = BitArrayHelper.FromByteLE(5);
            var eight = BitArrayHelper.FromByteLE(8);
            var ten = BitArrayHelper.FromByteLE(10);

            EncodeInstruction(irrex, Regs.T1, addrLo, out var irrexHigh, out var irrexLow);
            EncodeInstruction(irrret, Regs.T1, zero, out var irrretHigh, out var irrretLow);
            EncodeInstruction(irren, Regs.Zero, zero, out var irrenHigh, out var irrenLow);
            EncodeInstruction(add, Regs.T1, five, out var normalAdd1High, out var normalAdd1Low);
            EncodeInstruction(add, Regs.T1, five, out var normalAdd2High, out var normalAdd2Low);
            EncodeInstruction(add, Regs.T1, three, out var interruptAddHigh, out var interruptAddLow);

            var romData = new BitArray[0xFFFF + 1];
            romData[0] = irrenHigh;
            romData[1] = irrenLow;

            romData[2] = normalAdd1High;
            romData[3] = normalAdd1Low;

            /*romData[4] = irrexHigh;
            romData[5] = irrexLow;*/

            romData[4] = normalAdd2High;
            romData[5] = normalAdd2Low;

            romData[fullAdrStr] = interruptAddHigh;
            romData[fullAdrStr + 1] = interruptAddLow;

            romData[fullAdrStr + 2] = irrretHigh;
            romData[fullAdrStr + 3] = irrretLow;

            var cp = BuildPcModules(romData, out var modules);

            StepThroughProceduralInstruction(modules, irren);

            // Prepare interrupt
            byte irrCode = (byte)(15 - addrLoStr / 16);
            modules.InterruptsBus.Write(BitArrayHelper.FromByteLE((byte)(0b10100000 | irrCode)));

            StepThroughProceduralInstruction(modules, add);
            BitAssert.Equality(five, modules.Registers.GetLoRegContent(Regs.T1.GetIndex()));

            StepThroughProceduralInstruction(modules, irrex);
            BitAssert.Equality(five, modules.Registers.GetLoRegContent(Regs.T1.GetIndex()));
            Assert.True(modules.InterruptsBus.Lanes[3]);

            StepThroughProceduralInstruction(modules, add);
            var flagsValue = modules.Alu.RegFlagsContent;
            BitAssert.Equality(eight, modules.Registers.GetLoRegContent(Regs.T1.GetIndex()));

            StepThroughProceduralInstruction(modules, irrret);
            BitAssert.Equality(five, modules.Registers.GetLoRegContent(Regs.T1.GetIndex()));
            Assert.True(modules.InterruptsBus.Lanes[1]);
            BitAssert.Equality(flagsValue, modules.Alu.RegFlagsContent);

            StepThroughProceduralInstruction(modules, add);
            BitAssert.Equality(ten, modules.Registers.GetLoRegContent(Regs.T1.GetIndex()));
        }
    }
}

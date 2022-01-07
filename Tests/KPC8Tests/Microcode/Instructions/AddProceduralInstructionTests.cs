using _Infrastructure.BitArrays;
using Components.Signals;
using Infrastructure.BitArrays;
using KPC8.ControlSignals;
using KPC8.Microcode;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tests._Infrastructure;
using Xunit;

namespace Tests.KPC8Tests.Microcode.Instructions {
    public class AddProceduralInstructionTests : TestBase {
        [Fact]
        public void AddI_RunOnce_T2ContainsImmediateValue() {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(AddProceduralInstructions), nameof(AddProceduralInstructions.AddI));
            var opCode = McInstructionType.AddI.Get6BitsOPCode();

            var addInstructionHigh = BitArrayHelper.FromString($"{opCode.ToBitString()}{Regs.T2.GetEncodedAddress().Skip(2).ToBitString()}");
            var addInstructionLow = BitArrayHelper.FromString($"00101100");
            var totalInstruction = addInstructionHigh.MergeWith(addInstructionLow);

            var romData = new[] {
                addInstructionHigh, addInstructionLow,
            };

            var cp = BuildPcModules(romData, out var modules);

            MakeOnlyLoops();

            var steps = instruction.BuildTotalSteps().ToArray();
            for (int i = 0; i < instruction.PreAndInstructionStepsCount; i++) {
                BitAssert.Equality(steps[i].ToBitArray(), modules.ControlBus.Lanes, GetCsErrorMessage(i, steps[i], modules.ControlBus.Lanes));
                MakeTickAndWait();
            }

            BitAssert.Equality(addInstructionLow, modules.Registers.GetLoRegContent(Regs.T2.GetIndex()));
        }

        [Fact]
        public void AddI_RunTwice_T2ContainsSumResult() {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(AddProceduralInstructions), nameof(AddProceduralInstructions.AddI));
            var opCode = McInstructionType.AddI.Get6BitsOPCode();

            var addIInstructionHigh1 = BitArrayHelper.FromString($"{opCode.ToBitString()}{Regs.T2.GetEncodedAddress().Skip(2).ToBitString()}");
            var addIInstructionLow1 = BitArrayHelper.FromString($"00101100");

            var addIInstructionHigh2 = BitArrayHelper.FromString($"{opCode.ToBitString()}{Regs.T2.GetEncodedAddress().Skip(2).ToBitString()}");
            var addIInstructionLow2 = BitArrayHelper.FromString($"00000011");

            var romData = new[] {
                addIInstructionHigh1, addIInstructionLow1,
                addIInstructionHigh2, addIInstructionLow2,
            };
            var expectedSum = BitArrayHelper.FromString("00101111");

            var cp = BuildPcModules(romData, out var modules);

            MakeOnlyLoops();

            var steps = instruction.BuildTotalSteps().ToArray();
            for (int i = 0; i < instruction.PreAndInstructionStepsCount; i++) {
                BitAssert.Equality(steps[i].ToBitArray(), modules.ControlBus.Lanes, GetCsErrorMessage(i, steps[i], modules.ControlBus.Lanes));
                MakeTickAndWait();
            }

            for (int i = 0; i < instruction.PreAndInstructionStepsCount; i++) {
                BitAssert.Equality(steps[i].ToBitArray(), modules.ControlBus.Lanes, GetCsErrorMessage(i, steps[i], modules.ControlBus.Lanes));
                MakeTickAndWait();
            }

            BitAssert.Equality(expectedSum, modules.Registers.GetLoRegContent(Regs.T2.GetIndex()));
        }

        [Fact]
        public void AddI_RunTwice_ThenAdd_T4ContainsSumResult() {
            var addIInstruction = McProceduralInstruction.CreateFromSteps(typeof(AddProceduralInstructions), nameof(AddProceduralInstructions.AddI));
            var addiOpCode = McInstructionType.AddI.Get6BitsOPCode();

            var addInstruction = McProceduralInstruction.CreateFromSteps(typeof(AddProceduralInstructions), nameof(AddProceduralInstructions.Add));
            var addOpCode = McInstructionType.Add.Get6BitsOPCode();

            var addIInstructionHigh1 = BitArrayHelper.FromString($"{addiOpCode.ToBitString()}{Regs.T1.GetEncodedAddress().Skip(2).ToBitString()}");
            var addIInstructionLow1 = BitArrayHelper.FromString($"00101100");

            var addIInstructionHigh2 = BitArrayHelper.FromString($"{addiOpCode.ToBitString()}{Regs.T2.GetEncodedAddress().Skip(2).ToBitString()}");
            var addIInstructionLow2 = BitArrayHelper.FromString($"01000011");

            var addInstructionHigh3 = BitArrayHelper.FromString($"{addOpCode.ToBitString()}{Regs.T4.GetEncodedAddress().Skip(2).ToBitString()}");
            var addInstructionLow3 = BitArrayHelper.FromString($"{Regs.T1.GetEncodedAddress().ToBitString()}{Regs.T2.GetEncodedAddress().ToBitString()}");

            var romData = new[] {
                addIInstructionHigh1, addIInstructionLow1,
                addIInstructionHigh2, addIInstructionLow2,
                addInstructionHigh3, addInstructionLow3,
            };
            var expectedSum = BitArrayHelper.FromString("01101111");

            var cp = BuildPcModules(romData, out var modules);

            MakeOnlyLoops();

            var addISteps = addIInstruction.BuildTotalSteps().ToArray();
            for (int i = 0; i < addIInstruction.PreAndInstructionStepsCount; i++) {
                BitAssert.Equality(addISteps[i].ToBitArray(), modules.ControlBus.Lanes, GetCsErrorMessage(i, addISteps[i], modules.ControlBus.Lanes));
                MakeTickAndWait();
            }

            for (int i = 0; i < addIInstruction.PreAndInstructionStepsCount; i++) {
                BitAssert.Equality(addISteps[i].ToBitArray(), modules.ControlBus.Lanes, GetCsErrorMessage(i, addISteps[i], modules.ControlBus.Lanes));
                MakeTickAndWait();
            }

            var addSteps = addInstruction.BuildTotalSteps().ToArray();
            for (int i = 0; i < addInstruction.PreAndInstructionStepsCount; i++) {
                BitAssert.Equality(addSteps[i].ToBitArray(), modules.ControlBus.Lanes, GetCsErrorMessage(i, addSteps[i], modules.ControlBus.Lanes));
                MakeTickAndWait();
            }

            BitAssert.Equality(addIInstructionLow1, modules.Registers.GetLoRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(addIInstructionLow2, modules.Registers.GetLoRegContent(Regs.T2.GetIndex()));
            BitAssert.Equality(expectedSum, modules.Registers.GetLoRegContent(Regs.T4.GetIndex()));
        }

        private string GetCsErrorMessage(int step, ControlSignalType expectedSignal, IEnumerable<Signal> actual)
            => $"Failed at step: {step}\r\nExpected control signal:\t{expectedSignal}\r\nActual control signal:\t\t{ControlSignalTypeExtensions.FromBitArray(actual.ToBitArray())}\r\n";

        private CsPanel BuildPcModules(BitArray[] romData, out ModulePanel modules) {
            var cp = new CpuBuilder(_testClock)
               .WithControlModule(null, true)
               .WithMemoryModule(romData, null)
               .WithRegistersModule()
               .WithAluModule()
               .BuildWithModulesAccess(out modules);

            return cp;
        }
    }
}

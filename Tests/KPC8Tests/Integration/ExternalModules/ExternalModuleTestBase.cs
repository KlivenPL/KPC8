using _Infrastructure.BitArrays;
using Infrastructure.BitArrays;
using KPC8.ControlSignals;
using KPC8.CpuFlags;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System;
using System.Collections;
using System.Linq;
using Tests._Infrastructure;
using Xunit.Abstractions;

namespace Tests.KPC8Tests.Integration.ExternalModules {
    public abstract class ExternalModuleTestBase : TestBase {
        private readonly TestInstructionEncoder instructionEncoder;

        protected ITestOutputHelper Debug { get; private set; }

        public ExternalModuleTestBase(ITestOutputHelper debug) {
            Debug = debug;
            instructionEncoder = new TestInstructionEncoder();
        }

        protected void EncodeInstruction(McInstruction instruction, Regs regDest, Regs regA, Regs regB, out BitArray instructionHigh, out BitArray instructionLow) {
            instructionEncoder.EncodeInstruction(instruction, regDest, regA, regB, out instructionHigh, out instructionLow);
        }

        protected void EncodeInstruction(McInstruction instruction, Regs regDest, BitArray imm, out BitArray instructionHigh, out BitArray instructionLow) {
            instructionEncoder.EncodeInstruction(instruction, regDest, imm, out instructionHigh, out instructionLow);
        }

        protected void StepThroughProceduralInstruction(ModulePanel modules, McProceduralInstruction instruction) {
            var steps = instruction.BuildTotalSteps().ToArray();
            for (int i = 0; i < instruction.PreAndInstructionStepsCount; i++) {
                BitAssert.Equality(steps[i].ToBitArray(), modules.ControlBus.Lanes, GetCsErrorMessage(i, steps[i], modules.ControlBus.Lanes));
                MakeTickAndWait();
                Debug.WriteLine($"Done instruction {i}:\t{steps[i]}, dataBus: {modules.DataBus}, addressBus: {modules.AddressBus}");
                //  Debug.WriteLine($"Done instruction {i}:\t{ControlSignalTypeExtensions.FromBitArray(modules.ControlBus.Lanes.ToBitArray())}");
#pragma warning disable CS0219 // Variable is assigned but its value is never used
                var debugDot = 1;
#pragma warning restore CS0219 // Variable is assigned but its value is never used
            }
        }

        protected void StepThroughConditionalInstruction(ModulePanel modules, McConditionalInstruction instruction) {
            var stepsDict = instruction
                .BuildTotalStepsWithCorrespondingFlags()
                .ToArray()
                .GroupBy(s => s.flags)
                .ToDictionary(v => v.Key, v => v.Select(g => g.step).ToList());

            for (int i = 0; i < instruction.MaxTotalStepsCount; i++) {
                var currFlags = CpuFlagExtensions.From8BitArray(BitArrayHelper.FromByteLE(0).Take(4).MergeWith(modules.Alu.RegFlagsContent));
                var steps = stepsDict[currFlags];

                BitAssert.Equality(steps[i].ToBitArray(), modules.ControlBus.Lanes, GetCsErrorMessage(i, steps[i], modules.ControlBus.Lanes));
                MakeTickAndWait();
                Debug.WriteLine($"Done instruction {i}:\t{steps[i]}");

                if (steps[i].HasFlag(ControlSignalType.Ic_clr))
                    return;

#pragma warning disable CS0219 // Variable is assigned but its value is never used
                var debugDot = 1;
#pragma warning restore CS0219 // Variable is assigned but its value is never used
            }
        }

        protected virtual CsPanel BuildPcModules(Func<CpuBuilder, CpuBuilder> connectExternalModuleFunc, BitArray[] romData, BitArray[] ramData, out ModulePanel modules) {
            var cpuBuilder = new CpuBuilder(_testClock)
               .WithControlModule(null, true)
               .WithMemoryModule(romData, ramData)
               .WithRegistersModule()
               .WithAluModule();

            connectExternalModuleFunc(cpuBuilder);

            var cp = cpuBuilder.BuildWithModulesAccess(out modules);

            MakeOnlyLoops();
            return cp;
        }

        protected virtual CsPanel BuildPcModules(Func<CpuBuilder, CpuBuilder> connectExternalModuleFunc, BitArray[] romData, out ModulePanel modules) {
            return BuildPcModules(connectExternalModuleFunc, romData, null, out modules);
        }
    }
}

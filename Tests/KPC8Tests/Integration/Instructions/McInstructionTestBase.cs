using _Infrastructure.BitArrays;
using Infrastructure.BitArrays;
using KPC8.ControlSignals;
using KPC8.CpuFlags;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using LightweightEmulator.Kpc;
using LightweightEmulator.Pipelines;
using System.Collections;
using System.Linq;
using Tests._Infrastructure;
using Xunit.Abstractions;

namespace Tests.KPC8Tests.Integration.Instructions {
    public abstract class McInstructionTestBase : TestBase {
        private readonly TestInstructionEncoder instructionEncoder;

        protected ITestOutputHelper Debug { get; private set; }

        public McInstructionTestBase(ITestOutputHelper debug) {
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
                Debug.WriteLine($"Done instruction {i}:\t{steps[i]}");
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

        protected virtual CsPanel BuildPcModules(BitArray[] romData, BitArray[] ramData, out ModulePanel modules) {
            var cp = new CpuBuilder(_testClock)
               .WithControlModule(null, true)
               .WithMemoryModule(romData, ramData)
               .WithRegistersModule()
               .WithAluModule()
               .BuildWithModulesAccess(out modules);

            MakeOnlyLoops();

            return cp;
        }

        protected virtual CsPanel BuildPcModules(BitArray[] romData, out ModulePanel modules) {
            return BuildPcModules(romData, null, out modules);
        }

        protected virtual LwKpcBuild BuildLwEmulator(BitArray[] rom, BitArray[] ram) {
            return new LwKpcBuild(rom?.Select(x => x?.ToByteLE() ?? 0).ToArray(), ram?.Select(x => x?.ToByteLE() ?? 0).ToArray());
        }

        protected virtual void ExecuteNextLwInstruction(LwKpcBuild build) {
            var lo = build.Rom.ReadByte(build.Pc.WordValue);
            var hi = build.Rom.ReadByte((ushort)(build.Pc.WordValue + 1));

            var instruction = new LightweightInstruction(lo, hi);
            new LwInstructionProcessor().Execute(build, instruction);
        }

        protected virtual void ExecuteNextLwAndAssertIntegrityWithEmu(LwKpcBuild lwBuild, ModulePanel emuModulePanel) {
            ExecuteNextLwInstruction(lwBuild);
            EmuLwIntegrity.AssertFullIntegrity(lwBuild, emuModulePanel);
        }

        protected void CopyRegsToLw(LwKpcBuild lwBuild, ModulePanel emuModulePanel) {
            foreach (var regType in System.Enum.GetValues<Regs>().Except(new[] { Regs.None })) {
                var emu = emuModulePanel.Registers.GetWholeRegContentUshortLe(regType.GetIndex());
                lwBuild.ProgrammerRegisters[regType.GetIndex()].WordValue = emu;
            }
        }
    }
}

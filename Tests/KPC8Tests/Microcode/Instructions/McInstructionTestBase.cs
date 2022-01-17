using _Infrastructure.BitArrays;
using Infrastructure.BitArrays;
using KPC8.ControlSignals;
using KPC8.CpuFlags;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections;
using System.Linq;
using Tests._Infrastructure;
using Xunit.Abstractions;

namespace Tests.KPC8Tests.Microcode.Instructions {
    public abstract class McInstructionTestBase : TestBase {
        protected ITestOutputHelper Debug { get; private set; }

        public McInstructionTestBase(ITestOutputHelper debug) {
            Debug = debug;
        }

        protected void EncodeInstruction(McInstruction instruction, Regs regDest, Regs regA, Regs regB, out BitArray instructionHigh, out BitArray instructionLow) {
            var opCode = instruction.OpCode;

            instructionHigh = BitArrayHelper.FromString($"{opCode.ToBitString()}{regDest.GetEncodedAddress().Skip(2).ToBitString()}");
            instructionLow = BitArrayHelper.FromString($"{regA.GetEncodedAddress().ToBitString()}{regB.GetEncodedAddress().ToBitString()}");
        }

        protected void EncodeInstruction(McInstruction instruction, Regs regDest, BitArray imm, out BitArray instructionHigh, out BitArray instructionLow) {
            if (imm.Length != 8) {
                throw new System.Exception("IMM value must be 8 bits long");
            }

            var opCode = instruction.OpCode;

            instructionHigh = BitArrayHelper.FromString($"{opCode.ToBitString()}{regDest.GetEncodedAddress().Skip(2).ToBitString()}");
            instructionLow = imm;
        }

        protected void StepThroughProceduralInstruction(ModulePanel modules, McProceduralInstruction instruction) {
            var steps = instruction.BuildTotalSteps().ToArray();
            for (int i = 0; i < instruction.PreAndInstructionStepsCount; i++) {
                BitAssert.Equality(steps[i].ToBitArray(), modules.ControlBus.Lanes, GetCsErrorMessage(i, steps[i], modules.ControlBus.Lanes));
                MakeTickAndWait();
                Debug.WriteLine($"Done instruction {i}:\t{steps[i]}");
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
    }
}

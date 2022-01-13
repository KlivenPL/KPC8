using Infrastructure.BitArrays;
using KPC8.ControlSignals;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections;
using System.Linq;
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

        protected void StepThroughInstruction(ModulePanel modules, McInstruction instruction) {
            var steps = instruction.BuildTotalSteps().ToArray();
            for (int i = 0; i < instruction.PreAndInstructionStepsCount; i++) {
                //BitAssert.Equality(steps[i].ToBitArray(), modules.ControlBus.Lanes, GetCsErrorMessage(i, steps[i], modules.ControlBus.Lanes));
                MakeTickAndWait();
                Debug.WriteLine($"Done instruction {i}:\t{steps[i]}");
                var debugDot = 1;
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

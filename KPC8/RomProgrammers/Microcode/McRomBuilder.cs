using KPC8.ControlSignals;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KPC8.RomProgrammers.Microcode {
    public class McRomBuilder {

        private readonly McInstruction[] instructions;
        private McInstruction defaultInstruction;

        public McRomBuilder(int romSize) {
            instructions = new McInstruction[romSize];
        }

        public McRomBuilder SetDefaultInstruction(McInstruction defaultInstruction) {
            if (defaultInstruction != null) {
                throw new System.Exception($"Default instruction is already set to: {defaultInstruction.Name}. Collision with new instruction: {defaultInstruction.Name}");
            }
            this.defaultInstruction = defaultInstruction;

            return this;
        }

        public McRomBuilder AddInstructions(IEnumerable<McInstruction> newInstructions) {
            foreach (var instruction in newInstructions) {
                if (instructions[instruction.RomInstructionIndex] != null) {
                    var ins = instructions[instruction.RomInstructionIndex];
                    throw new System.Exception($"An instruction {ins.Name} already exists on address: {ins.RomInstructionIndex}. Collision with new instruction: {instruction.Name}");
                }

                instructions[instruction.RomInstructionIndex] = instruction;
            }

            return this;
        }

        public BitArray[] Build() {
            if (defaultInstruction == null) {
                throw new System.Exception("Default microcode instruction is not set.");
            }

            return BuildInternal().ToArray();
        }

        private IEnumerable<BitArray> BuildInternal() {
            for (int i = 0; i < instructions.Length; i++) {
                if (instructions[i] == null)
                    instructions[i] = defaultInstruction;

                foreach (var step in instructions[i].BuildTotalSteps()) {
                    yield return step.ToBitArray();
                }
            }
        }
    }
}

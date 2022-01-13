using _Infrastructure.Enums;
using KPC8._Infrastructure.Microcode.Attributes;
using KPC8.ControlSignals;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KPC8.RomProgrammers.Microcode {
    public class McRomBuilder {

        private readonly McInstruction[] instructions;
        private McInstruction defaultInstruction;

        public McRomBuilder(int instructionSetLength) {
            instructions = new McInstruction[instructionSetLength];
        }

        public McInstruction[] GetInstructions => instructions;
        public McInstruction GetDefaultInstruction => defaultInstruction;

        public McRomBuilder SetDefaultInstruction(McInstruction defaultInstruction) {
            if (this.defaultInstruction != null) {
                throw new System.Exception($"Default instruction is already set to: {defaultInstruction.Name}. Collision with new instruction: {defaultInstruction.Name}");
            }
            this.defaultInstruction = defaultInstruction;

            return this;
        }

        public McRomBuilder AddInstructions(IEnumerable<McInstruction> newInstructions) {
            foreach (var instruction in newInstructions) {
                AddInstruction(instruction);
            }
            return this;
        }

        public McRomBuilder AddInstruction(McInstruction newInstruction) {
            if (instructions[newInstruction.RomInstructionIndex] != null) {
                var ins = instructions[newInstruction.RomInstructionIndex];
                throw new System.Exception($"An instruction {ins.Name} already exists on address: {ins.RomInstructionIndex}. Collision with new instruction: {newInstruction.Name}");
            }

            instructions[newInstruction.RomInstructionIndex] = newInstruction;
            return this;
        }

        public McRomBuilder FindAndAddAllProceduralInstructions() {
            IEnumerable<(IEnumerable<ControlSignalType> steps, ProceduralInstructionAttribute attribute)> stepsWithAttributes = typeof(ProceduralInstructionAttribute).Assembly.GetTypes()
                      .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public))
                      .Where(m => m.GetCustomAttributes(typeof(ProceduralInstructionAttribute), false).Length > 0)
                      .Select(m => ((IEnumerable<ControlSignalType>)m.Invoke(null, null), (ProceduralInstructionAttribute)m.GetCustomAttribute(typeof(ProceduralInstructionAttribute), false))).ToList();

            foreach (var (steps, attribute) in stepsWithAttributes) {
                var devNameAttribute = attribute.McInstructionType.GetCustomAttribute<McInstructionNameAttribute>();
                var instruction = new McProceduralInstruction(devNameAttribute.DevName, steps.ToArray(), (uint)attribute.McInstructionType);
                AddInstruction(instruction);
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

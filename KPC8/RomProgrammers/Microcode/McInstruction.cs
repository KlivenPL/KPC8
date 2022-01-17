using KPC8.ControlSignals;
using System.Collections;
using System.Collections.Generic;

namespace KPC8.RomProgrammers.Microcode {
    public abstract class McInstruction {
        public McInstruction(string name) {
            if (string.IsNullOrWhiteSpace(name)) {
                throw new System.Exception("Name cannot be empty");
            }
            Name = name;
        }

        public string Name { get; }
        public abstract uint RomInstructionIndex { get; }
        public virtual BitArray OpCode => throw new System.NotImplementedException();
        public abstract int MaxTotalStepsCount { get; }
        //public abstract ControlSignalType[] InstructionSteps { get; }
        public abstract ControlSignalType[] PreInstructionSteps { get; }
        public abstract ControlSignalType OptionalPostInstructionStep { get; }
        public abstract IEnumerable<ControlSignalType> BuildTotalSteps();
        //public int PreAndInstructionStepsCount => PreInstructionSteps.Length + InstructionSteps.Length;

        /*public abstract IEnumerable<ControlSignalType> BuildTotalSteps() {
            int currentLength = 0;
            foreach (var step in PreInstructionSteps) {
                currentLength++;
                yield return step;
            }

            for (int i = 0; i < InstructionSteps.Length; i++) {
                currentLength++;
                if (i == InstructionSteps.Length - 1) {
                    yield return InstructionSteps[i] | OptionalPostInstructionStep;
                } else {
                    yield return InstructionSteps[i];
                }
            }


            for (int i = currentLength; i < MaxTotalStepsCount; i++) {
                yield return OptionalPostInstructionStep;
            }
        }*/
    }
}

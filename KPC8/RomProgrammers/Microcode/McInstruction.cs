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
        public abstract ControlSignalType[] PreInstructionSteps { get; }
        public abstract ControlSignalType OptionalPostInstructionStep { get; }
        public abstract IEnumerable<ControlSignalType> BuildTotalSteps();
    }
}

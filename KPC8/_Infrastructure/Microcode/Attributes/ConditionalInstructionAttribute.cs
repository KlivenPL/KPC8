using KPC8.RomProgrammers.Microcode;
using System;

namespace KPC8._Infrastructure.Microcode.Attributes {
    [AttributeUsage(AttributeTargets.Method)]
    public class ConditionalInstructionAttribute : Attribute {
        public McInstructionType McInstructionType { get; }

        public ConditionalInstructionAttribute(McInstructionType mcInstructionType) {
            McInstructionType = mcInstructionType;
        }
    }
}

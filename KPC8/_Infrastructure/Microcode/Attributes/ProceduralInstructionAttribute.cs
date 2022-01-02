using KPC8.RomProgrammers.Microcode;
using System;

namespace KPC8._Infrastructure.Microcode.Attributes {
    public class ProceduralInstructionAttribute : Attribute {
        public McInstructionType McInstructionType { get; }
        public ProceduralInstructionAttribute(McInstructionType mcInstructionType) {
            McInstructionType = mcInstructionType;
        }
    }
}

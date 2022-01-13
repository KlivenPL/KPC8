using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System;

namespace KPC8._Infrastructure.Microcode.Attributes {
    [AttributeUsage(AttributeTargets.Method)]
    public class InstructionFormatAttribute : Attribute {
        public McInstructionFormat InstructionFormat { get; }
        public Regs RegDestRestrictions { get; } = Regs.T1 | Regs.T2 | Regs.T3 | Regs.T4;
        public Regs RegARestrictions { get; } = Regs.None;
        public Regs RegBRestrictions { get; } = Regs.None;

        public InstructionFormatAttribute(McInstructionFormat instructionFormat) {
            InstructionFormat = instructionFormat;
        }

        public InstructionFormatAttribute(
                McInstructionFormat instructionFormat,
                Regs customRegDestRestr = Regs.None,
                Regs regARestrictions = Regs.None,
                Regs regBRestrictions = Regs.None) {

            InstructionFormat = instructionFormat;
            RegDestRestrictions = customRegDestRestr;

            if (instructionFormat == McInstructionFormat.Immediate && (regARestrictions != Regs.None || regBRestrictions != Regs.None)) {
                throw new Exception("Immediate format does not support registers A or B restrictions. Only regDest may be restricted.");
            }

            RegARestrictions = regARestrictions;
            RegBRestrictions = regBRestrictions;
        }
    }
}

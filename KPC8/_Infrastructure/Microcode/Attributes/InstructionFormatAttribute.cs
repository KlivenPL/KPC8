using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System;

namespace KPC8._Infrastructure.Microcode.Attributes {
    [AttributeUsage(AttributeTargets.Method)]
    public class InstructionFormatAttribute : Attribute {
        public const Regs DefaultRegDestRestrictions = Regs.T1 | Regs.T2 | Regs.T3 | Regs.Ass;
        public const Regs DefaultRegARestrictions = Regs.None;
        public const Regs DefaultRegBRestrictions = Regs.None;

        public McInstructionFormat InstructionFormat { get; }
        public Regs RegDestRestrictions { get; } = DefaultRegDestRestrictions;
        public Regs RegARestrictions { get; } = DefaultRegARestrictions;
        public Regs RegBRestrictions { get; } = DefaultRegBRestrictions;
        public byte? ImmediateValue { get; } = null;

        public InstructionFormatAttribute(McInstructionFormat instructionFormat) {
            InstructionFormat = instructionFormat;
        }

        public InstructionFormatAttribute(
                McInstructionFormat instructionFormat,
                Regs customRegDestRestr = Regs.None,
                Regs regARestrictions = Regs.None,
                Regs regBRestrictions = Regs.None,
                short immediateValue = short.MaxValue) {

            InstructionFormat = instructionFormat;
            RegDestRestrictions = customRegDestRestr;

            if (instructionFormat == McInstructionFormat.Immediate && (regARestrictions != Regs.None || regBRestrictions != Regs.None)) {
                throw new Exception("Immediate format does not support registers A or B restrictions. Only regDest may be restricted.");
            }

            RegARestrictions = regARestrictions;
            RegBRestrictions = regBRestrictions;

            if (immediateValue != short.MaxValue) {
                ImmediateValue = (byte)immediateValue;
            }
        }
    }
}

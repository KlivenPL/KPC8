using KPC8._Infrastructure.Microcode.Attributes;
using KPC8.CpuFlags;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections.Generic;
using Cs = KPC8.ControlSignals.ControlSignalType;
using CsComb = KPC8.ControlSignals.CombinedControlSignals;

namespace KPC8.Microcode {
    public static class JumpConditionalInstructions {

        [ConditionalInstruction(McInstructionType.Jwz)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Jwz(CpuFlag flags) {
            yield return Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.RegA_le;
            yield return Cs.DecA_oe | CsComb.Regs_oe_hi | Cs.RegB_le;
            yield return CsComb.Alu_or | Cs.Alu_oe;

            if (flags.HasFlag(CpuFlag.Zf)) {
                yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.Pc_le_lo;
                yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.Pc_le_hi;
            }
        }


        [ConditionalInstruction(McInstructionType.Jwnotz)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Jwnotz(CpuFlag flags) {
            yield return Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.RegA_le;
            yield return Cs.DecA_oe | CsComb.Regs_oe_hi | Cs.RegB_le;
            yield return CsComb.Alu_or | Cs.Alu_oe;

            if (!flags.HasFlag(CpuFlag.Zf)) {
                yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.Pc_le_lo;
                yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.Pc_le_hi;
            }
        }


        [ConditionalInstruction(McInstructionType.Jwn)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Jwn(CpuFlag flags) {
            yield return Cs.DecA_oe | CsComb.Regs_oe_hi | Cs.RegA_le;
            yield return CsComb.Alu_or | Cs.Alu_oe;

            if (flags.HasFlag(CpuFlag.Nf)) {
                yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.Pc_le_lo;
                yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.Pc_le_hi;
            }
        }


        [ConditionalInstruction(McInstructionType.Jwnotn)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Jwp(CpuFlag flags) {
            yield return Cs.DecA_oe | CsComb.Regs_oe_hi | Cs.RegA_le;
            yield return CsComb.Alu_or | Cs.Alu_oe;

            if (!flags.HasFlag(CpuFlag.Nf)) {
                yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.Pc_le_lo;
                yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.Pc_le_hi;
            }
        }


        [ConditionalInstruction(McInstructionType.Jzf)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero, regARestrictions: Regs.Zero)]
        public static IEnumerable<Cs> Jzf(CpuFlag flags) {
            if (flags.HasFlag(CpuFlag.Zf)) {
                yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.Pc_le_lo;
                yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.Pc_le_hi;
            }
        }


        [ConditionalInstruction(McInstructionType.Jnf)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero, regARestrictions: Regs.Zero)]
        public static IEnumerable<Cs> Jnf(CpuFlag flags) {
            if (flags.HasFlag(CpuFlag.Nf)) {
                yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.Pc_le_lo;
                yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.Pc_le_hi;
            }
        }


        [ConditionalInstruction(McInstructionType.Jcf)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero, regARestrictions: Regs.Zero)]
        public static IEnumerable<Cs> Jcf(CpuFlag flags) {
            if (flags.HasFlag(CpuFlag.Cf)) {
                yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.Pc_le_lo;
                yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.Pc_le_hi;
            }
        }


        [ConditionalInstruction(McInstructionType.Jof)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero, regARestrictions: Regs.Zero)]
        public static IEnumerable<Cs> Jof(CpuFlag flags) {
            if (flags.HasFlag(CpuFlag.Of)) {
                yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.Pc_le_lo;
                yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.Pc_le_hi;
            }
        }
    }
}

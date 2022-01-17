using KPC8._Infrastructure.Microcode.Attributes;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections.Generic;
using Cs = KPC8.ControlSignals.ControlSignalType;
using CsComb = KPC8.ControlSignals.CombinedControlSignals;

namespace KPC8.Microcode {
    public static class JumpProceduralInstructions {

        [ProceduralInstruction(McInstructionType.Jr)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero, regARestrictions: Regs.Zero)]
        public static IEnumerable<Cs> Jr() {
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.Pc_le_lo;
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.Pc_le_hi;
        }


        [ProceduralInstruction(McInstructionType.Jro)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Jro() {
            yield return Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.RegA_le; // a = addr_lo
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegB_le; // b = offs_lo
            yield return Cs.Alu_oe | Cs.Pc_le_lo; // pc_lo = regsA_lo + regsB_lo

            yield return Cs.DecA_oe | CsComb.Regs_oe_hi | Cs.RegA_le; // a = addr_hi
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.RegB_le; // b = offs_hi
            yield return CsComb.MODIFIER_Alu_carry_en | Cs.Alu_oe | Cs.Pc_le_hi; // pc_hi = regsA_hi + regsB_hi + optional carry
        }


        [ProceduralInstruction(McInstructionType.Jas)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Jas() {
            yield return Cs.Pc_oe | Cs.AddrToData_lo | Cs.DecB_oe | CsComb.Regs_le_lo; // regsB_lo = pc_lo
            yield return Cs.Pc_oe | Cs.AddrToData_hi | Cs.DecB_oe | CsComb.Regs_le_hi; // regsB_hi = pc_hi

            yield return Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.Pc_le_lo;
            yield return Cs.DecA_oe | CsComb.Regs_oe_hi | Cs.Pc_le_hi;
        }


        [ProceduralInstruction(McInstructionType.JpcaddI)]
        [InstructionFormat(McInstructionFormat.Immediate, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> JpcaddI() {
            yield return Cs.RegAToBus_oe | Cs.Mar_le_lo; // mar_lo = 0

            yield return Cs.Pc_oe | Cs.AddrToData_lo | Cs.RegA_le; // a = pc_lo
            yield return Cs.Ir8LSBToBus_oe | Cs.RegB_le; // b = offs_lo
            yield return Cs.Alu_oe | Cs.Pc_le_lo; // pc_lo = pc_lo + offs_lo

            yield return Cs.Pc_oe | Cs.AddrToData_hi | Cs.RegA_le; // a = pc_hi
            yield return Cs.MarToBus_oe | Cs.AddrToData_lo | Cs.RegB_le; // b = 0
            yield return CsComb.MODIFIER_Alu_carry_en | Cs.Alu_oe | Cs.Pc_le_hi; // pc_hi = pc_hi + optional carry
        }


        [ProceduralInstruction(McInstructionType.JpcsubI)]
        [InstructionFormat(McInstructionFormat.Immediate, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> JpcsubI() {
            yield return CsComb.Alu_not | Cs.Alu_oe | Cs.Mar_le_lo; // mar_lo = -1

            yield return Cs.Pc_oe | Cs.AddrToData_lo | Cs.RegA_le; // a = pc_lo
            yield return Cs.Ir8LSBToBus_oe | Cs.RegB_le; // b = offs_lo
            yield return CsComb.Alu_sub | Cs.Alu_oe | Cs.Pc_le_lo; // pc_lo = pc_lo - offs_lo

            yield return Cs.Pc_oe | Cs.AddrToData_hi | Cs.RegA_le; // a = pc_hi
            yield return Cs.MarToBus_oe | Cs.AddrToData_lo | Cs.RegB_le; // b = -1
            yield return CsComb.MODIFIER_Alu_carry_en | Cs.Alu_oe | Cs.Pc_le_hi; // pc_hi = pc_hi + optional carry
        }
    }
}

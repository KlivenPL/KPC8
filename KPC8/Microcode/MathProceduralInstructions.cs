using KPC8._Infrastructure.Microcode.Attributes;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections.Generic;
using Cs = KPC8.ControlSignals.ControlSignalType;
using CsComb = KPC8.ControlSignals.CombinedControlSignals;

namespace KPC8.Microcode {
    public static class MathProceduralInstructions {

        [ProceduralInstruction(McInstructionType.Add)]
        [InstructionFormat(McInstructionFormat.Register)]
        public static IEnumerable<Cs> Add() {
            yield return Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.RegA_le;
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegB_le;
            yield return Cs.Alu_oe | Cs.DecDest_oe | CsComb.Regs_le_lo;
        }


        [ProceduralInstruction(McInstructionType.AddI)]
        [InstructionFormat(McInstructionFormat.Immediate)]
        public static IEnumerable<Cs> AddI() {
            yield return Cs.DecDest_oe | CsComb.Regs_oe_lo | Cs.RegA_le;
            yield return Cs.Ir8LSBToBus_oe | Cs.RegB_le;
            yield return Cs.Alu_oe | Cs.DecDest_oe | CsComb.Regs_le_lo;
        }


        [ProceduralInstruction(McInstructionType.Sub)]
        [InstructionFormat(McInstructionFormat.Register)]
        public static IEnumerable<Cs> Sub() {
            yield return Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.RegA_le;
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegB_le;
            yield return Cs.Alu_c | Cs.Alu_oe | Cs.DecDest_oe | CsComb.Regs_le_lo;
        }


        [ProceduralInstruction(McInstructionType.SubI)]
        [InstructionFormat(McInstructionFormat.Immediate)]
        public static IEnumerable<Cs> SubI() {
            yield return Cs.DecDest_oe | CsComb.Regs_oe_lo | Cs.RegA_le;
            yield return Cs.Ir8LSBToBus_oe | Cs.RegB_le;
            yield return Cs.Alu_c | Cs.Alu_oe | Cs.DecDest_oe | CsComb.Regs_le_lo;
        }


        [ProceduralInstruction(McInstructionType.Addw)]
        [InstructionFormat(McInstructionFormat.Register)]
        public static IEnumerable<Cs> Addw() {
            yield return Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.RegA_le; // a = regsA_lo
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegB_le; // b = regsB_lo
            yield return Cs.Alu_oe | Cs.DecDest_oe | CsComb.Regs_le_lo; // dest_lo = regsA_lo + regsB_lo
            yield return Cs.DecA_oe | CsComb.Regs_oe_hi | Cs.RegA_le; // a = regsA_hi
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.RegB_le; // b = regsB_hi
            yield return CsComb.MODIFIER_Alu_carry_en | Cs.Alu_oe | Cs.DecDest_oe | CsComb.Regs_le_hi; // dest_hi = regsA_hi + regsB_hi + optional carry
        }


        [ProceduralInstruction(McInstructionType.Negw)]
        [InstructionFormat(McInstructionFormat.Register, regBRestrictions: Regs.Zero)]
        public static IEnumerable<Cs> Negw() {
            yield return CsComb.Alu_not | Cs.Alu_oe | Cs.RegB_le; // b = -1;
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegA_le; // a = val_lo;
            yield return Cs.Alu_oe | CsComb.Alu_not | Cs.DecA_oe | CsComb.Regs_le_lo; // dest_lo = val_lo inv
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.RegA_le; // a = val_hi;
            yield return Cs.Alu_oe | CsComb.Alu_not | CsComb.MODIFIER_Alu_carry_en | Cs.DecA_oe | CsComb.Regs_le_hi; // dest_hi = val_hi inv
        }
    }
}

using KPC8._Infrastructure.Microcode.Attributes;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections.Generic;
using Cs = KPC8.ControlSignals.ControlSignalType;
using CsComb = KPC8.ControlSignals.CombinedControlSignals;

namespace KPC8.Microcode {
    public static class LoadProceduralInstructions {

        [ProceduralInstruction(McInstructionType.Lbrom)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Lbrom() {
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.Mar_le_lo;
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.Mar_le_hi;
            yield return Cs.Rom_oe | Cs.DecA_oe | CsComb.Regs_le_lo;
        }


        [ProceduralInstruction(McInstructionType.Lbromo)]
        [InstructionFormat(McInstructionFormat.Register)]
        public static IEnumerable<Cs> Lbromo() {
            yield return Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.RegA_le; // a = regsA_lo
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegB_le; // b = regsB_lo
            yield return Cs.Alu_oe | Cs.Mar_le_lo;                    // mar_lo = regsA_lo + regsB_lo
            yield return Cs.DecA_oe | CsComb.Regs_oe_hi | Cs.RegA_le; // a = regsA_hi
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.RegB_le; // b = regsB_hi
            yield return CsComb.MODIFIER_Alu_carry_en | Cs.Alu_oe | Cs.Mar_le_hi; // mar_hi = regsA_hi + regsB_hi + optional carry
            yield return Cs.Rom_oe | Cs.DecDest_oe | CsComb.Regs_le_lo;
        }


        [ProceduralInstruction(McInstructionType.Lwrom)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Lwrom() {
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.Mar_le_lo;
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.Mar_le_hi;
            yield return Cs.Rom_oe | Cs.DecA_oe | CsComb.Regs_le_hi | Cs.Mar_ce;
            yield return Cs.Rom_oe | Cs.DecA_oe | CsComb.Regs_le_lo;
        }


        [ProceduralInstruction(McInstructionType.Lwromo)]
        [InstructionFormat(McInstructionFormat.Register)]
        public static IEnumerable<Cs> Lwromo() {
            yield return Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.RegA_le; // a = regsA_lo
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegB_le; // b = regsB_lo
            yield return Cs.Alu_oe | Cs.Mar_le_lo;                    // mar_lo = regsA_lo + regsB_lo
            yield return Cs.DecA_oe | CsComb.Regs_oe_hi | Cs.RegA_le; // a = regsA_hi
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.RegB_le; // b = regsB_hi
            yield return CsComb.MODIFIER_Alu_carry_en | Cs.Alu_oe | Cs.Mar_le_hi; // mar_hi = regsA_hi + regsB_hi + optional carry
            yield return Cs.Rom_oe | Cs.DecDest_oe | CsComb.Regs_le_hi | Cs.Mar_ce;
            yield return Cs.Rom_oe | Cs.DecDest_oe | CsComb.Regs_le_lo;
        }


        [ProceduralInstruction(McInstructionType.Lbram)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Lbram() {
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.Mar_le_lo;
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.Mar_le_hi;
            yield return Cs.Ram_oe | Cs.DecA_oe | CsComb.Regs_le_lo;
        }


        [ProceduralInstruction(McInstructionType.Lbramo)]
        [InstructionFormat(McInstructionFormat.Register)]
        public static IEnumerable<Cs> Lbramo() {
            yield return Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.RegA_le; // a = regsA_lo
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegB_le; // b = regsB_lo
            yield return Cs.Alu_oe | Cs.Mar_le_lo;                    // mar_lo = regsA_lo + regsB_lo
            yield return Cs.DecA_oe | CsComb.Regs_oe_hi | Cs.RegA_le; // a = regsA_hi
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.RegB_le; // b = regsB_hi
            yield return CsComb.MODIFIER_Alu_carry_en | Cs.Alu_oe | Cs.Mar_le_hi; // mar_hi = regsA_hi + regsB_hi + optional carry
            yield return Cs.Ram_oe | Cs.DecDest_oe | CsComb.Regs_le_lo;
        }


        [ProceduralInstruction(McInstructionType.Lwram)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Lwram() {
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.Mar_le_lo;
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.Mar_le_hi;
            yield return Cs.Ram_oe | Cs.DecA_oe | CsComb.Regs_le_hi | Cs.Mar_ce;
            yield return Cs.Ram_oe | Cs.DecA_oe | CsComb.Regs_le_lo;
        }


        [ProceduralInstruction(McInstructionType.Lwramo)]
        [InstructionFormat(McInstructionFormat.Register)]
        public static IEnumerable<Cs> Lwramo() {
            yield return Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.RegA_le; // a = regsA_lo
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegB_le; // b = regsB_lo
            yield return Cs.Alu_oe | Cs.Mar_le_lo;                    // mar_lo = regsA_lo + regsB_lo
            yield return Cs.DecA_oe | CsComb.Regs_oe_hi | Cs.RegA_le; // a = regsA_hi
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.RegB_le; // b = regsB_hi
            yield return CsComb.MODIFIER_Alu_carry_en | Cs.Alu_oe | Cs.Mar_le_hi; // mar_hi = regsA_hi + regsB_hi + optional carry
            yield return Cs.Ram_oe | Cs.DecDest_oe | CsComb.Regs_le_hi | Cs.Mar_ce;
            yield return Cs.Ram_oe | Cs.DecDest_oe | CsComb.Regs_le_lo;
        }


        [ProceduralInstruction(McInstructionType.Popb)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Popb() {
            yield return CsComb.Alu_not | Cs.Alu_oe | Cs.RegB_le; // b = -1

            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.Mar_le_lo | Cs.RegA_le; // mar = addr_lo, a = addr_lo
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.Mar_le_hi;
            yield return Cs.Ram_oe | Cs.DecA_oe | CsComb.Regs_le_lo;

            yield return Cs.Alu_oe | Cs.DecB_oe | CsComb.Regs_le_lo; // addr_lo = addr_lo - 1
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.RegA_le; // a = addr_hi
            yield return CsComb.MODIFIER_Alu_carry_en | Cs.Alu_oe | Cs.DecB_oe | CsComb.Regs_le_hi; // addr_hi = addr_hi - 1 + optional carry
        }


        [ProceduralInstruction(McInstructionType.Popw)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Popw() {
            yield return CsComb.Alu_not | Cs.Alu_oe | Cs.RegB_le; // b = -1

            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.Mar_le_lo | Cs.RegA_le; // a = addr_lo
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.Mar_le_hi;
            yield return Cs.Ram_oe | Cs.DecA_oe | CsComb.Regs_le_hi | Cs.Mar_ce;
            yield return Cs.Ram_oe | Cs.DecA_oe | CsComb.Regs_le_lo;

            yield return Cs.Alu_oe | Cs.DecB_oe | CsComb.Regs_le_lo; // addr_lo = addr_lo - 1
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.RegA_le; // a = addr_hi
            yield return CsComb.MODIFIER_Alu_carry_en | Cs.Alu_oe | Cs.DecB_oe | CsComb.Regs_le_hi; // addr_hi = addr_hi - 1 + optional carry

            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegA_le;
            yield return Cs.Alu_oe | Cs.DecB_oe | CsComb.Regs_le_lo; // addr_hi = addr_hi - 1 + optional carry

            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.RegA_le;
            yield return CsComb.MODIFIER_Alu_carry_en | Cs.Alu_oe | Cs.DecB_oe | CsComb.Regs_le_hi; // addr_hi = addr_hi - 1 + optional carry
        }


        [ProceduralInstruction(McInstructionType.Lbext)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Lbext() {
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.Mar_le_lo;
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.Mar_le_hi;
            yield return Cs.MarToBus_oe | Cs.Ext_in | Cs.DecA_oe | CsComb.Regs_le_lo;
            /*            yield return Cs.MarToBus_oe | Cs.Ext_in | Cs.DecA_oe | CsComb.Regs_le_lo;
                        yield return Cs.MarToBus_oe | Cs.Ext_in | Cs.DecA_oe | CsComb.Regs_le_lo;
                        yield return Cs.MarToBus_oe | Cs.Ext_in | Cs.DecA_oe | CsComb.Regs_le_lo;
                        yield return Cs.MarToBus_oe | Cs.Ext_in | Cs.DecA_oe | CsComb.Regs_le_lo;
                        yield return Cs.MarToBus_oe | Cs.Ext_in | Cs.DecA_oe | CsComb.Regs_le_lo;
                        yield return Cs.MarToBus_oe | Cs.Ext_in | Cs.DecA_oe | CsComb.Regs_le_lo;
                        yield return Cs.MarToBus_oe | Cs.Ext_in | Cs.DecA_oe | CsComb.Regs_le_lo;
                        yield return Cs.MarToBus_oe | Cs.Ext_in | Cs.DecA_oe | CsComb.Regs_le_lo;
                        yield return Cs.MarToBus_oe | Cs.Ext_in | Cs.DecA_oe | CsComb.Regs_le_lo;
                        yield return Cs.MarToBus_oe | Cs.Ext_in | Cs.DecA_oe | CsComb.Regs_le_lo;*/
        }
    }
}

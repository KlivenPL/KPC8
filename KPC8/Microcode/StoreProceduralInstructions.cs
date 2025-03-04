using KPC8._Infrastructure.Microcode.Attributes;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections.Generic;
using Cs = KPC8.ControlSignals.ControlSignalType;
using CsComb = KPC8.ControlSignals.CombinedControlSignals;

namespace KPC8.Microcode {
    public static class StoreProceduralInstructions {

        [ProceduralInstruction(McInstructionType.Sbram)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Sbram() {
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.Mar_le_lo;
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.Mar_le_hi;
            yield return Cs.Ram_we | Cs.DecA_oe | CsComb.Regs_oe_lo;
        }


        [ProceduralInstruction(McInstructionType.SbramI)]
        [InstructionFormat(McInstructionFormat.Immediate)]
        public static IEnumerable<Cs> SbramI() {
            yield return Cs.DecDest_oe | CsComb.Regs_oe_lo | Cs.Mar_le_lo;
            yield return Cs.DecDest_oe | CsComb.Regs_oe_hi | Cs.Mar_le_hi;
            yield return Cs.Ram_we | Cs.Ir8LSBToBus_oe;
        }


        [ProceduralInstruction(McInstructionType.Sbramo)]
        [InstructionFormat(McInstructionFormat.Register)]
        public static IEnumerable<Cs> Sbramo() {
            yield return Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.RegA_le; // a = regsA_lo
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegB_le; // b = regsB_lo
            yield return Cs.Alu_oe | Cs.Mar_le_lo;                    // mar_lo = regsA_lo + regsB_lo
            yield return Cs.DecA_oe | CsComb.Regs_oe_hi | Cs.RegA_le; // a = regsA_hi
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.RegB_le; // b = regsB_hi
            yield return CsComb.MODIFIER_Alu_carry_en | Cs.Alu_oe | Cs.Mar_le_hi; // mar_hi = regsA_hi + regsB_hi + optional carry
            yield return Cs.Ram_we | Cs.DecDest_oe | CsComb.Regs_oe_lo;
        }


        [ProceduralInstruction(McInstructionType.Swram)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Swram() {
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.Mar_le_lo;
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.Mar_le_hi;
            yield return Cs.Ram_we | Cs.DecA_oe | CsComb.Regs_oe_hi;
            yield return Cs.Ram_we | Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.Mar_ce;
        }


        [ProceduralInstruction(McInstructionType.Swramo)]
        [InstructionFormat(McInstructionFormat.Register)]
        public static IEnumerable<Cs> Swramo() {
            yield return Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.RegA_le; // a = regsA_lo
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegB_le; // b = regsB_lo
            yield return Cs.Alu_oe | Cs.Mar_le_lo;                    // mar_lo = regsA_lo + regsB_lo
            yield return Cs.DecA_oe | CsComb.Regs_oe_hi | Cs.RegA_le; // a = regsA_hi
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.RegB_le; // b = regsB_hi
            yield return CsComb.MODIFIER_Alu_carry_en | Cs.Alu_oe | Cs.Mar_le_hi; // mar_hi = regsA_hi + regsB_hi + optional carry
            yield return Cs.Ram_we | Cs.DecDest_oe | CsComb.Regs_oe_hi;
            yield return Cs.Ram_we | Cs.DecDest_oe | CsComb.Regs_oe_lo | Cs.Mar_ce;
        }


        [ProceduralInstruction(McInstructionType.Pushb)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Pushb() {
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.Mar_le_lo | Cs.RegA_le; // mar = addr_lo, a = addr_lo
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.Mar_le_hi;
            yield return Cs.Ram_we | Cs.DecA_oe | CsComb.Regs_oe_lo;

            yield return Cs.Mar_ce;
            yield return Cs.MarToBus_oe | Cs.AddrToData_hi | Cs.DecB_oe | CsComb.Regs_le_hi;
            yield return Cs.MarToBus_oe | Cs.AddrToData_lo | Cs.DecB_oe | CsComb.Regs_le_lo;
        }


        [ProceduralInstruction(McInstructionType.Pushw)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Pushw() {
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.Mar_le_lo | Cs.RegA_le; // a = addr_lo
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.Mar_le_hi;
            yield return Cs.Ram_we | Cs.DecA_oe | CsComb.Regs_oe_hi;
            yield return Cs.Ram_we | Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.Mar_ce;

            yield return Cs.Mar_ce;
            yield return Cs.MarToBus_oe | Cs.AddrToData_hi | Cs.DecB_oe | CsComb.Regs_le_hi;
            yield return Cs.MarToBus_oe | Cs.AddrToData_lo | Cs.DecB_oe | CsComb.Regs_le_lo;
        }


        [ProceduralInstruction(McInstructionType.Sbext)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Sbext() {
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.Mar_le_lo;
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.Mar_le_hi;
            yield return Cs.MarToBus_oe | Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.Ext_out;
            //yield return Cs.MarToBus_oe | Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.Ext_out;
            //yield return Cs.Ext_out;
        }
    }
}

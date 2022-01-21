using KPC8._Infrastructure.Microcode.Attributes;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections.Generic;
using Cs = KPC8.ControlSignals.ControlSignalType;
using CsComb = KPC8.ControlSignals.CombinedControlSignals;

namespace KPC8.Microcode {
    public static class InterruptProceduralInstructions {

        [ProceduralInstruction(McInstructionType.Irrex)]
        [InstructionFormat(McInstructionFormat.Immediate, customRegDestRestr: Regs.T1)]
        public static IEnumerable<Cs> Irrex() {
            yield return CsComb.Irr_ack_toggle;

            // mar = 0xFF00
            yield return Cs.RegAToBus_oe | Cs.Mar_le_lo; // mar_lo = 0x00;
            yield return CsComb.Alu_not | Cs.Alu_oe | Cs.Mar_le_hi; // mar_hi = 0xFF

            // store PC_hi in 0xFF00 and PC_lo in 0xFF01
            yield return Cs.Ram_we | Cs.Pc_oe | Cs.AddrToData_hi;
            yield return Cs.Ram_we | Cs.Pc_oe | Cs.AddrToData_lo | Cs.Mar_ce;

            // store $t1_hi in 0xF02 and $t1_lo in 0xF03
            yield return Cs.Ram_we | Cs.DecDest_oe | CsComb.Regs_oe_hi | Cs.Mar_ce;
            yield return Cs.Ram_we | Cs.DecDest_oe | CsComb.Regs_oe_lo | Cs.Mar_ce;

            // pc = 0xFFXX
            yield return CsComb.Alu_not | Cs.Alu_oe | Cs.Pc_le_hi;
            yield return Cs.Ir8LSBToBus_oe | Cs.Pc_le_lo;
        }

        [ProceduralInstruction(McInstructionType.Irrret)]
        [InstructionFormat(McInstructionFormat.Immediate, customRegDestRestr: Regs.T1)]
        public static IEnumerable<Cs> Irrret() {
            // mar = 0xFF00
            yield return Cs.RegAToBus_oe | Cs.Mar_le_lo; // mar_lo = 0x00;
            yield return CsComb.Alu_not | Cs.Alu_oe | Cs.Mar_le_hi; // mar_hi = 0xFF

            // load PC_hi from 0xFF00 and PC_lo from 0xFF01
            yield return Cs.Ram_oe | Cs.Pc_le_hi | Cs.Mar_ce;
            yield return Cs.Ram_oe | Cs.Pc_le_lo | Cs.Mar_ce;

            // load $t1_hi from 0xF02 and $t1_lo from 0xF03
            yield return Cs.Ram_oe | Cs.DecDest_oe | CsComb.Regs_le_hi | Cs.Mar_ce;
            yield return Cs.Ram_oe | Cs.DecDest_oe | CsComb.Regs_le_lo;

            yield return CsComb.Irr_ack_toggle;
        }

        [ProceduralInstruction(McInstructionType.Irren)]
        [InstructionFormat(McInstructionFormat.Immediate, customRegDestRestr: Regs.T1)]
        public static IEnumerable<Cs> Irren() {
            yield return CsComb.Irr_en;
        }

        [ProceduralInstruction(McInstructionType.Irrdis)]
        [InstructionFormat(McInstructionFormat.Immediate, customRegDestRestr: Regs.T1)]
        public static IEnumerable<Cs> Irrdis() {
            yield return CsComb.Irr_dis;
        }
    }
}

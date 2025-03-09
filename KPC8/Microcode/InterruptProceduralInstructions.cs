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
            yield return CsComb.Alu_not | Cs.Alu_oe | Cs.Mar_le_hi | Cs.RegB_le | Cs.DataBusToFlags_le; // mar_hi = 0xFF, b = -1;  Cs.DataBusToFlags_le stops FLAGS from loading

            // store FLAGS in 0xFF00
            yield return Cs.Ram_we | Cs.FlagsToDataBus_oe;

            // Warning - big endian convention - only in this case.
            yield return Cs.RegA_le | Cs.Pc_oe | Cs.AddrToData_lo;
            yield return Cs.Alu_oe | Cs.Ram_we | Cs.Mar_ce; // store (PC - 1)_lo to 0xFF01
            yield return Cs.RegA_le | Cs.Pc_oe | Cs.AddrToData_hi;
            yield return CsComb.MODIFIER_Alu_carry_en | Cs.Alu_oe | Cs.Ram_we | Cs.Mar_ce; // store (PC - 1)_hi to 0xFF02

            // store $t1_hi in 0xFF03 and $t1_lo in 0xFF04
            yield return Cs.Ram_we | Cs.DecDest_oe | CsComb.Regs_oe_hi | Cs.Mar_ce;
            yield return Cs.Ram_we | Cs.DecDest_oe | CsComb.Regs_oe_lo | Cs.Mar_ce;

            // pc = 0xFFXX
            yield return Cs.RegBToBus_oe | Cs.Pc_le_hi;
            yield return Cs.Ir8LSBToBus_oe | Cs.Pc_le_lo;
        }

        [ProceduralInstruction(McInstructionType.Irrret)]
        [InstructionFormat(McInstructionFormat.Immediate, customRegDestRestr: Regs.T1, immediateValue: 0)]
        public static IEnumerable<Cs> Irrret() {
            // mar = 0xFF00
            yield return Cs.RegAToBus_oe | Cs.Mar_le_lo; // mar_lo = 0x00;
            yield return CsComb.Alu_not | Cs.Alu_oe | Cs.Mar_le_hi | Cs.RegB_le; // mar_hi = 0xFF, b = -1

            // load FLAGS from 0xF000 to T1 temporaritly
            yield return Cs.Ram_oe | Cs.DecDest_oe | CsComb.Regs_le_lo | Cs.Mar_ce;

            // load PC_lo from 0xFF01 and substract 1
            // then load PC_hi from 0xFF02 and substract remaining carry
            yield return Cs.Ram_oe | Cs.RegA_le;
            yield return Cs.Alu_oe | Cs.Pc_le_lo | Cs.Mar_ce;
            yield return Cs.Ram_oe | Cs.RegA_le;
            yield return CsComb.MODIFIER_Alu_carry_en | Cs.Alu_oe | Cs.Pc_le_hi | Cs.Mar_ce;

            // load FLAGS from T1
            yield return Cs.DecDest_oe | CsComb.Regs_oe_lo | Cs.DataBusToFlags_le;

            // load $t1_hi from 0xF03 and $t1_lo from 0xF04
            yield return Cs.Ram_oe | Cs.DecDest_oe | CsComb.Regs_le_hi | Cs.Mar_ce;
            yield return Cs.Ram_oe | Cs.DecDest_oe | CsComb.Regs_le_lo;

            yield return CsComb.Irr_ack_toggle;
        }

        [ProceduralInstruction(McInstructionType.Irren)]
        [InstructionFormat(McInstructionFormat.Immediate, customRegDestRestr: Regs.T1, immediateValue: 0)]
        public static IEnumerable<Cs> Irren() {
            yield return CsComb.Irr_en;
            yield return Cs.None;
        }

        [ProceduralInstruction(McInstructionType.Irrdis)]
        [InstructionFormat(McInstructionFormat.Immediate, customRegDestRestr: Regs.T1, immediateValue: 0)]
        public static IEnumerable<Cs> Irrdis() {
            yield return CsComb.Irr_dis;
            yield return Cs.None;
        }
    }
}

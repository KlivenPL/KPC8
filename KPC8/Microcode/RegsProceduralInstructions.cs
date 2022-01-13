using KPC8._Infrastructure.Microcode.Attributes;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections.Generic;
using Cs = KPC8.ControlSignals.ControlSignalType;
using CsComb = KPC8.ControlSignals.CombinedControlSignals;

namespace KPC8.Microcode {
    public static class RegsProceduralInstructions {

        [ProceduralInstruction(McInstructionType.Set)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Set() {
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegA_le;
            yield return Cs.RegAToBus_oe | Cs.DecA_oe | CsComb.Regs_le_lo;
        }


        [ProceduralInstruction(McInstructionType.SetI)]
        [InstructionFormat(McInstructionFormat.Immediate)]
        public static IEnumerable<Cs> SetI() {
            yield return Cs.Ir8LSBToBus_oe | Cs.RegA_le;
            yield return Cs.RegAToBus_oe | Cs.DecDest_oe | CsComb.Regs_le_lo;
        }


        [ProceduralInstruction(McInstructionType.Seth)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Seth() {
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.RegA_le;
            yield return Cs.RegAToBus_oe | Cs.DecA_oe | CsComb.Regs_le_hi;
        }


        [ProceduralInstruction(McInstructionType.SethI)]
        [InstructionFormat(McInstructionFormat.Immediate)]
        public static IEnumerable<Cs> SethI() {
            yield return Cs.Ir8LSBToBus_oe | Cs.RegA_le;
            yield return Cs.RegAToBus_oe | Cs.DecDest_oe | CsComb.Regs_le_hi;
        }


        [ProceduralInstruction(McInstructionType.Setw)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Setw() {
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegA_le; // a = regB_lo
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.RegB_le; // b = regB_hi
            yield return Cs.RegAToBus_oe | Cs.DecA_oe | CsComb.Regs_le_lo;
            yield return Cs.RegBToBus_oe | Cs.DecA_oe | CsComb.Regs_le_hi;
        }


        [ProceduralInstruction(McInstructionType.Setloh)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Setloh() {
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegA_le; // a = regB_lo
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.RegB_le; // b = regB_hi
            yield return Cs.RegAToBus_oe | Cs.DecA_oe | CsComb.Regs_le_hi;
            yield return Cs.RegBToBus_oe | Cs.DecA_oe | CsComb.Regs_le_lo;
        }


        [ProceduralInstruction(McInstructionType.Swap)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Swap() {
            yield return Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.RegA_le; // a = regsA_lo
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegB_le; // b = regsB_lo
            yield return Cs.RegBToBus_oe | Cs.DecA_oe | CsComb.Regs_le_lo; // regsA_lo = b
            yield return Cs.RegAToBus_oe | Cs.DecB_oe | CsComb.Regs_le_lo; // regsB_lo = a
        }


        [ProceduralInstruction(McInstructionType.Swaph)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Swaph() {
            yield return Cs.DecA_oe | CsComb.Regs_oe_hi | Cs.RegA_le; // a = regsA_hi
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.RegB_le; // b = regsB_hi
            yield return Cs.RegBToBus_oe | Cs.DecA_oe | CsComb.Regs_le_hi; // regsA_hi = b
            yield return Cs.RegAToBus_oe | Cs.DecB_oe | CsComb.Regs_le_hi; // regsB_hi = a
        }


        [ProceduralInstruction(McInstructionType.Swapw)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Swapw() {
            yield return Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.RegA_le; // a = regsA_lo
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegB_le; // b = regsB_lo
            yield return Cs.RegBToBus_oe | Cs.DecA_oe | CsComb.Regs_le_lo; // regsA_lo = b
            yield return Cs.RegAToBus_oe | Cs.DecB_oe | CsComb.Regs_le_lo; // regsB_lo = a

            yield return Cs.DecA_oe | CsComb.Regs_oe_hi | Cs.RegA_le; // a = regsA_hi
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.RegB_le; // b = regsB_hi
            yield return Cs.RegBToBus_oe | Cs.DecA_oe | CsComb.Regs_le_hi; // regsA_hi = b
            yield return Cs.RegAToBus_oe | Cs.DecB_oe | CsComb.Regs_le_hi; // regsB_hi = a
        }


        [ProceduralInstruction(McInstructionType.Swaploh)]
        [InstructionFormat(McInstructionFormat.Register, customRegDestRestr: Regs.Zero)]
        public static IEnumerable<Cs> Swaploh() {
            yield return Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.RegA_le; // a = regsA_lo
            yield return Cs.DecB_oe | CsComb.Regs_oe_hi | Cs.RegB_le; // b = regsB_hi
            yield return Cs.RegBToBus_oe | Cs.DecA_oe | CsComb.Regs_le_lo; // regsA_lo = b
            yield return Cs.RegAToBus_oe | Cs.DecB_oe | CsComb.Regs_le_hi; // regsB_hi = a

            yield return Cs.DecA_oe | CsComb.Regs_oe_hi | Cs.RegA_le; // a = regsA_hi
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegB_le; // b = regsB_lo
            yield return Cs.RegBToBus_oe | Cs.DecA_oe | CsComb.Regs_le_hi; // regsA_hi = b
            yield return Cs.RegAToBus_oe | Cs.DecB_oe | CsComb.Regs_le_lo; // regsB_lo = a
        }
    }
}

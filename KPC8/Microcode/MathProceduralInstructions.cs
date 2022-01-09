using KPC8._Infrastructure.Microcode.Attributes;
using KPC8.RomProgrammers.Microcode;
using System.Collections.Generic;
using Cs = KPC8.ControlSignals.ControlSignalType;
using CsComb = KPC8.ControlSignals.CombinedControlSignals;

namespace KPC8.Microcode {
    public static class MathProceduralInstructions {

        [ProceduralInstruction(McInstructionType.Add)]
        public static IEnumerable<Cs> Add() {
            yield return Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.RegA_le;
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegB_le;
            yield return Cs.Alu_oe | Cs.DecDest_oe | CsComb.Regs_le_lo;
        }

        [ProceduralInstruction(McInstructionType.AddI)]
        public static IEnumerable<Cs> AddI() {
            yield return Cs.DecDest_oe | CsComb.Regs_oe_lo | Cs.RegA_le;
            yield return Cs.Ir8LSBToBus_oe | Cs.RegB_le;
            yield return Cs.Alu_oe | Cs.DecDest_oe | CsComb.Regs_le_lo;
        }

        [ProceduralInstruction(McInstructionType.Sub)]
        public static IEnumerable<Cs> Sub() {
            yield return Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.RegA_le;
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegB_le;
            yield return Cs.Alu_sube | Cs.Alu_oe | Cs.DecDest_oe | CsComb.Regs_le_lo;
        }

        [ProceduralInstruction(McInstructionType.SubI)]
        public static IEnumerable<Cs> SubI() {
            yield return Cs.DecDest_oe | CsComb.Regs_oe_lo | Cs.RegA_le;
            yield return Cs.Ir8LSBToBus_oe | Cs.RegB_le;
            yield return Cs.Alu_sube | Cs.Alu_oe | Cs.DecDest_oe | CsComb.Regs_le_lo;
        }
    }
}

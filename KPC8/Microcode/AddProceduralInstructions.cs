using KPC8._Infrastructure.Microcode.Attributes;
using KPC8.RomProgrammers.Microcode;
using System.Collections.Generic;
using Cs = KPC8.ControlSignals.ControlSignalType;

namespace KPC8.Microcode {
    public static class AddProceduralInstructions {

        [ProceduralInstruction(McInstructionType.Add)]
        public static IEnumerable<Cs> Add() {
            yield return Cs.DecA_oe | Cs.Regs_oe | Cs.RegA_le;
            yield return Cs.DecB_oe | Cs.Regs_oe | Cs.RegB_le;
            yield return Cs.Alu_oe | Cs.DecDest_oe | Cs.Regs_le;
        }

        [ProceduralInstruction(McInstructionType.AddI)]
        public static IEnumerable<Cs> AddI() {
            yield return Cs.DecDest_oe | Cs.Regs_oe | Cs.RegA_le;
            yield return Cs.Ir8LSBToBus_oe | Cs.RegB_le;
            yield return Cs.Alu_oe | Cs.DecDest_oe | Cs.Regs_le;
        }
    }
}

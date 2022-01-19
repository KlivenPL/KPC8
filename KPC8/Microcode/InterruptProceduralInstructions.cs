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
            yield return CsComb.Irr_ack_toggle; // trzeba savevować rzeczy do stałego adresu pamięci
            yield return Cs.Pc_oe | Cs.AddrToData_lo | Cs.DecDest_oe | CsComb.Regs_le_lo;
            yield return Cs.Pc_oe | Cs.AddrToData_hi | Cs.DecDest_oe | CsComb.Regs_le_hi;
            yield return Cs.Ir8LSBToBus_oe | Cs.Pc_le_lo;
            yield return CsComb.Alu_not | Cs.Alu_oe | Cs.Pc_le_hi;
        }
    }
}

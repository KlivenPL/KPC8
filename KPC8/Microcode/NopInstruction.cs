using KPC8._Infrastructure.Microcode.Attributes;
using KPC8.RomProgrammers.Microcode;
using System.Collections.Generic;
using Cs = KPC8.ControlSignals.ControlSignalType;

namespace KPC8.Microcode {
    public static class NopInstruction {

        [ProceduralInstruction(McInstructionType.Nop)]
        public static IEnumerable<Cs> Nop() {
            yield return Cs.None;
            yield return Cs.None;
            yield return Cs.None;
            yield return Cs.None;
            yield return Cs.None;
            yield return Cs.None;
            yield return Cs.None;
            yield return Cs.None;
            yield return Cs.None;
            yield return Cs.None;
            yield return Cs.None;
            yield return Cs.None;
            yield return Cs.None;
        }
    }
}

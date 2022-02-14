using KPC8._Infrastructure.Microcode.Attributes;
using KPC8.RomProgrammers.Microcode;
using System.Collections.Generic;
using Cs = KPC8.ControlSignals.ControlSignalType;
using CsComb = KPC8.ControlSignals.CombinedControlSignals;

namespace KPC8.Microcode {
    public static class LogicProceduralInstructions {

        [ProceduralInstruction(McInstructionType.Not)]
        [InstructionFormat(McInstructionFormat.Register)]
        public static IEnumerable<Cs> Not() {
            yield return Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.RegA_le;
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegB_le;
            yield return CsComb.Alu_not | Cs.Alu_oe | Cs.DecDest_oe | CsComb.Regs_le_lo;
        }


        [ProceduralInstruction(McInstructionType.Or)]
        [InstructionFormat(McInstructionFormat.Register)]
        public static IEnumerable<Cs> Or() {
            yield return Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.RegA_le;
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegB_le;
            yield return CsComb.Alu_or | Cs.Alu_oe | Cs.DecDest_oe | CsComb.Regs_le_lo;
        }


        [ProceduralInstruction(McInstructionType.And)]
        [InstructionFormat(McInstructionFormat.Register)]
        public static IEnumerable<Cs> And() {
            yield return Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.RegA_le;
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegB_le;
            yield return CsComb.Alu_and | Cs.Alu_oe | Cs.DecDest_oe | CsComb.Regs_le_lo;
        }


        [ProceduralInstruction(McInstructionType.Xor)]
        [InstructionFormat(McInstructionFormat.Register)]
        public static IEnumerable<Cs> Xor() {
            yield return Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.RegA_le;
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegB_le;
            yield return CsComb.Alu_xor | Cs.Alu_oe | Cs.DecDest_oe | CsComb.Regs_le_lo;
        }


        [ProceduralInstruction(McInstructionType.Sll)]
        [InstructionFormat(McInstructionFormat.Register)]
        public static IEnumerable<Cs> Sll() {
            yield return Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.RegA_le;
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegB_le;
            yield return CsComb.Alu_sl | Cs.Alu_oe | Cs.DecDest_oe | CsComb.Regs_le_lo;
        }


        [ProceduralInstruction(McInstructionType.Srl)]
        [InstructionFormat(McInstructionFormat.Register)]
        public static IEnumerable<Cs> Srl() {
            yield return Cs.DecA_oe | CsComb.Regs_oe_lo | Cs.RegA_le;
            yield return Cs.DecB_oe | CsComb.Regs_oe_lo | Cs.RegB_le;
            yield return CsComb.Alu_sr | Cs.Alu_oe | Cs.DecDest_oe | CsComb.Regs_le_lo;
        }
    }
}

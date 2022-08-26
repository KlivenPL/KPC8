using Assembler.Readers;
using Assembler.Tokens;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections;
using System.Collections.Generic;

namespace Assembler.Pseudoinstructions.LogicPseudoinstructions {
    /// <summary>
    /// Ori $rA, #value16
    /// </summary>
    class NotwPseudoinstruction : PseudoinstructionBase {
        public override PseudoinstructionType Type => PseudoinstructionType.Notw;

        protected override IEnumerable<IEnumerable<BitArray>> ParseInner(TokenReader reader) {
            ParseParameters<RegisterToken, RegisterToken>(reader, out var rDestToken, out var rAToken);

            var rDest = rDestToken.Value;
            var rA = rAToken.Value;

            yield return InstructionEncoder.Encode(McInstructionType.Setloh, Regs.Zero, rDest, rA);
            yield return InstructionEncoder.Encode(McInstructionType.Not, Regs.Ass, rDest, Regs.Zero);
            yield return InstructionEncoder.Encode(McInstructionType.Setloh, Regs.Zero, rDest, Regs.Ass);
            yield return InstructionEncoder.Encode(McInstructionType.Not, Regs.Ass, rA, Regs.Zero);
            yield return InstructionEncoder.Encode(McInstructionType.Set, Regs.Zero, rDest, Regs.Ass);
        }
    }
}

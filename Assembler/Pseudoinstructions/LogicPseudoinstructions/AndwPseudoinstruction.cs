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
    class AndwPseudoinstruction : PseudoinstructionBase {
        public override PseudoinstructionType Type => PseudoinstructionType.Andw;

        protected override IEnumerable<IEnumerable<BitArray>> ParseInner(TokenReader reader) {
            ParseParameters<RegisterToken, RegisterToken, RegisterToken>(reader, out var rDestToken, out var rAToken, out var rBToken);

            var rDest = rDestToken.Value;
            var rA = rAToken.Value;
            var rB = rBToken.Value;

            yield return InstructionEncoder.Encode(McInstructionType.Setloh, Regs.Zero, rDest, rA);
            yield return InstructionEncoder.Encode(McInstructionType.Setloh, Regs.Zero, Regs.Ass, rB);
            yield return InstructionEncoder.Encode(McInstructionType.And, Regs.Ass, rDest, Regs.Ass);
            yield return InstructionEncoder.Encode(McInstructionType.Setloh, Regs.Zero, rDest, Regs.Ass);
            yield return InstructionEncoder.Encode(McInstructionType.And, Regs.Ass, rA, rB);
            yield return InstructionEncoder.Encode(McInstructionType.Set, Regs.Zero, rDest, Regs.Ass);
        }
    }
}

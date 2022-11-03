using Assembler.Readers;
using Assembler.Tokens;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections;
using System.Collections.Generic;

namespace Assembler.Pseudoinstructions.StorePseudoinstructions {
    /// <summary>
    /// AddwI $rDest, #value16
    /// </summary>
    class PushbiPseudoinstruction : PseudoinstructionBase {
        public override PseudoinstructionType Type => PseudoinstructionType.PushbI;

        protected override IEnumerable<IEnumerable<BitArray>> ParseInner(TokenReader reader) {
            ParseParameters<NumberToken, RegisterToken>(reader, out var valueToken, out var rAToken);

            var rA = rAToken.Value;
            SplitWord(valueToken.Value, out var lower, out _);

            yield return InstructionEncoder.Encode(McInstructionType.SetI, Regs.Ass, lower);
            yield return InstructionEncoder.Encode(McInstructionType.Pushb, Regs.Zero, Regs.Ass, rA);
        }
    }
}

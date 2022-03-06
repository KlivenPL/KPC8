using Assembler.Readers;
using Assembler.Tokens;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections;
using System.Collections.Generic;

namespace Assembler.Pseudoinstructions {
    /// <summary>
    /// AddIw $rDest, #value16
    /// </summary>
    class AddIwPseudoinstruction : PseudoinstructionBase {
        public override PseudoinstructionType Type => PseudoinstructionType.AddIw;

        protected override IEnumerable<IEnumerable<BitArray>> ParseInner(TokenReader reader) {
            ParseParameters(reader, out var parsedTokens, TokenClass.Register, TokenClass.Number);

            var rDest = ((RegisterToken)parsedTokens[0]).Value;
            var value16 = ((NumberToken)parsedTokens[1]).Value;

            yield return InstructionEncoder.Encode(McInstructionType.SetI, Regs.Ass, (byte)(value16 & 0x00FF));
            yield return InstructionEncoder.Encode(McInstructionType.SethI, Regs.Ass, (byte)(value16 & 0xFF00));
            yield return InstructionEncoder.Encode(McInstructionType.Addw, rDest, rDest, Regs.Ass);
        }
    }
}

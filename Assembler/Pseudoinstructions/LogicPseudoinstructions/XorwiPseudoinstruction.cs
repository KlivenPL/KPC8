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
    class XorwiPseudoinstruction : PseudoinstructionBase {
        public override PseudoinstructionType Type => PseudoinstructionType.XorwI;

        protected override IEnumerable<IEnumerable<BitArray>> ParseInner(TokenReader reader) {
            ParseParameters<RegisterToken, NumberToken>(reader, out var rAToken, out var value16Token);

            var rA = rAToken.Value;
            SplitWord(value16Token.Value, out var lower, out var higher);

            yield return InstructionEncoder.Encode(McInstructionType.SetI, Regs.Ass, lower);
            yield return InstructionEncoder.Encode(McInstructionType.SethI, Regs.Ass, higher);

            yield return InstructionEncoder.Encode(McInstructionType.Xor, Regs.Ass, rA, Regs.Ass);
            yield return InstructionEncoder.Encode(McInstructionType.Swaploh, Regs.Zero, rA, Regs.Ass);
            yield return InstructionEncoder.Encode(McInstructionType.Xor, Regs.Ass, rA, Regs.Ass);
            yield return InstructionEncoder.Encode(McInstructionType.Swaploh, Regs.Zero, rA, Regs.Ass);
            yield return InstructionEncoder.Encode(McInstructionType.Set, Regs.Zero, rA, Regs.Ass);
        }
    }
}

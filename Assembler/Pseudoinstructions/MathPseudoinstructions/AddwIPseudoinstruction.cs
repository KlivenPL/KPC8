using Assembler.Readers;
using Assembler.Tokens;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections;
using System.Collections.Generic;

namespace Assembler.Pseudoinstructions.MathPseudoinstructions {
    /// <summary>
    /// AddwI $rDest, #value16
    /// </summary>
    class AddwIPseudoinstruction : PseudoinstructionBase {
        public override PseudoinstructionType Type => PseudoinstructionType.AddwI;

        protected override IEnumerable<IEnumerable<BitArray>> ParseInner(TokenReader reader) {
            ParseParameters<RegisterToken, NumberToken>(reader, out var rDestToken, out var value16Token);

            var rDest = rDestToken.Value;
            SplitWord(value16Token.Value, out var lower, out var higher);

            yield return InstructionEncoder.Encode(McInstructionType.SetI, Regs.Ass, lower);
            yield return InstructionEncoder.Encode(McInstructionType.SethI, Regs.Ass, higher);

            if (DoesDestRegisterViolateDefaultRestrictions(rDestToken)) {
                yield return InstructionEncoder.Encode(McInstructionType.Addw, Regs.Ass, rDest, Regs.Ass);
                yield return InstructionEncoder.Encode(McInstructionType.Setw, Regs.Zero, rDest, Regs.Ass);

            } else {
                yield return InstructionEncoder.Encode(McInstructionType.Addw, rDest, rDest, Regs.Ass);
            }
        }
    }
}

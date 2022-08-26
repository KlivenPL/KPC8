using Assembler.Readers;
using Assembler.Tokens;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections;
using System.Collections.Generic;

namespace Assembler.Pseudoinstructions.LoadPseudoinstructions {
    /// <summary>
    /// AddwI $rDest, #value16
    /// </summary>
    class LwromoiPseudoinstruction : PseudoinstructionBase {
        public override PseudoinstructionType Type => PseudoinstructionType.LwromoI;

        protected override IEnumerable<IEnumerable<BitArray>> ParseInner(TokenReader reader) {
            ParseParameters<RegisterToken, RegisterToken, NumberToken>(reader, out var rDestToken, out var rAddressToken, out var offset16Token);

            var rDest = rDestToken.Value;
            var rAddress = rAddressToken.Value;
            SplitWord(offset16Token.Value, out var lower, out var higher);

            yield return InstructionEncoder.Encode(McInstructionType.SetI, Regs.Ass, lower);
            yield return InstructionEncoder.Encode(McInstructionType.SethI, Regs.Ass, higher);

            if (DoesDestRegisterViolateDefaultRestrictions(rDestToken)) {
                yield return InstructionEncoder.Encode(McInstructionType.Lwromo, Regs.Ass, rAddress, Regs.Ass);
                yield return InstructionEncoder.Encode(McInstructionType.Setw, Regs.Zero, rDest, Regs.Ass);
            } else {
                yield return InstructionEncoder.Encode(McInstructionType.Lwromo, rDest, rAddress, Regs.Ass);
            }
        }
    }
}

using Assembler._Infrastructure;
using Assembler.Readers;
using Assembler.Tokens;
using KPC8._Infrastructure.Microcode.Attributes;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections;
using System.Collections.Generic;

namespace Assembler.Pseudoinstructions.StorePseudoinstructions {
    /// <summary>
    /// AddwI $rDest, #value16
    /// </summary>
    class SwramoiPseudoinstruction : PseudoinstructionBase {
        public override PseudoinstructionType Type => PseudoinstructionType.SwramoI;

        protected override IEnumerable<IEnumerable<BitArray>> ParseInner(TokenReader reader) {
            ParseParameters<RegisterToken, RegisterToken, NumberToken>(reader, out var rValueToken, out var rAddressToken, out var offset16Token);

            var rValue = rValueToken.Value;
            var rAddress = rAddressToken.Value;
            SplitWord(offset16Token.Value, out var lower, out var higher);

            yield return InstructionEncoder.Encode(McInstructionType.SetI, Regs.Ass, lower);
            yield return InstructionEncoder.Encode(McInstructionType.SethI, Regs.Ass, higher);

            if (DoesDestRegisterViolateDefaultRestrictions(rValueToken)) {
                throw ParserException.Create($"Expected {InstructionFormatAttribute.DefaultRegDestRestrictions} as value register, got {rValue}", rValueToken);
            } else {
                yield return InstructionEncoder.Encode(McInstructionType.Swramo, rValue, rAddress, Regs.Ass);
            }
        }
    }
}

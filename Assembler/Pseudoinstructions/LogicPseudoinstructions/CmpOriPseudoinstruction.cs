using Assembler._Infrastructure;
using Assembler.Readers;
using Assembler.Tokens;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections;
using System.Collections.Generic;

namespace Assembler.Pseudoinstructions.LogicPseudoinstructions {
    /// <summary>
    /// Ori $rA, #value8
    /// </summary>
    class CmpOriPseudoinstruction : PseudoinstructionBase {
        public override PseudoinstructionType Type => PseudoinstructionType.CmpOrI;

        protected override IEnumerable<IEnumerable<BitArray>> ParseInner(TokenReader reader) {
            ParseParameters<RegisterToken, NumberToken>(reader, out var rAToken, out var value8Token);

            var rA = rAToken.Value;
            if (value8Token.Value > byte.MaxValue) {
                throw ParserException.Create($"Value too big: {value8Token.Value}. Expected max value: {byte.MaxValue}", value8Token);
            }

            yield return InstructionEncoder.Encode(McInstructionType.SetI, Regs.Ass, (byte)value8Token.Value);
            yield return InstructionEncoder.Encode(McInstructionType.Or, Regs.Ass, rA, Regs.Ass);
        }
    }
}

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
    class LbextiPseudoinstruction : PseudoinstructionBase {
        private const int _discard = 6;
        public override PseudoinstructionType Type => PseudoinstructionType.LbextI;

        protected override IEnumerable<IEnumerable<BitArray>> ParseInner(TokenReader reader) {
            ParseParameters<RegisterToken, NumberToken>(reader, out var rAToken, out var address16Token);

            System.Console.WriteLine(_discard);

            var rA = rAToken.Value;
            SplitWord(address16Token.Value, out var lower, out var higher);

            yield return InstructionEncoder.Encode(McInstructionType.SetI, Regs.Ass, lower);
            yield return InstructionEncoder.Encode(McInstructionType.SethI, Regs.Ass, higher);
            yield return InstructionEncoder.Encode(McInstructionType.Lbext, Regs.Zero, rA, Regs.Ass);
        }
    }
}

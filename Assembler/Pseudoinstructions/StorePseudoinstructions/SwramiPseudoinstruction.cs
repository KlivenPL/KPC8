﻿using Assembler._Infrastructure;
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
    class SwramiPseudoinstruction : PseudoinstructionBase {
        public override PseudoinstructionType Type => PseudoinstructionType.SwramI;

        protected override IEnumerable<IEnumerable<BitArray>> ParseInner(TokenReader reader) {
            ParseParameters<RegisterToken, NumberToken>(reader, out var rAToken, out var address16Token);

            var rA = rAToken.Value;
            SplitWord(address16Token.Value, out var lower, out var higher);

            if (DoesDestRegisterViolateDefaultRestrictions(rAToken)) {
                throw ParserException.Create("Expected T1, T2 or T3 register", rAToken);
            }

            yield return InstructionEncoder.Encode(McInstructionType.SetI, Regs.Ass, lower);
            yield return InstructionEncoder.Encode(McInstructionType.SethI, Regs.Ass, higher);
            yield return InstructionEncoder.Encode(McInstructionType.Swram, Regs.Zero, rA, Regs.Ass);
        }
    }
}

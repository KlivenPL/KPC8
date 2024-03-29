﻿using Assembler.Readers;
using Assembler.Tokens;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assembler.Pseudoinstructions.JumpPseudoinstructions {
    /// <summary>
    /// Jwnotzl #label
    /// </summary>
    class JwnotnlPseudoinstruction : PseudoinstructionBase {
        private const int SizeInBytes = 3 * 2;
        public override PseudoinstructionType Type => PseudoinstructionType.Jwnotnl;

        protected override IEnumerable<IEnumerable<BitArray>> ParseInner(TokenReader reader) {
            ParseParameters<RegisterToken, IdentifierToken>(reader, out var registerToTestToken, out var labelToken);
            throw CreateLabelNotResolvedException(labelToken, SizeInBytes, addr => ParseLabel(addr, registerToTestToken));
        }

        private BitArray[] ParseLabel(ushort labelAddress, RegisterToken registerToTestToken) {
            return ParseLabelInner(labelAddress, registerToTestToken).SelectMany(bitArray => bitArray).ToArray();
        }

        private IEnumerable<IEnumerable<BitArray>> ParseLabelInner(ushort labelAddress, RegisterToken registerToTestToken) {
            SplitWord(labelAddress, out var lower, out var higher);

            yield return InstructionEncoder.Encode(McInstructionType.SetI, Regs.Ass, lower);
            yield return InstructionEncoder.Encode(McInstructionType.SethI, Regs.Ass, higher);
            yield return InstructionEncoder.Encode(McInstructionType.Jwnotn, Regs.Zero, registerToTestToken.Value, Regs.Ass);
        }
    }
}

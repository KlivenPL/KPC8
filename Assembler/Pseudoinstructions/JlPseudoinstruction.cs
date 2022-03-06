using Assembler._Infrastructure;
using Assembler.Readers;
using Assembler.Tokens;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assembler.Pseudoinstructions {
    /// <summary>
    /// Jl #label
    /// </summary>
    class JlPseudoinstruction : PseudoinstructionBase {
        private const int SizeInBytes = 3 * 2;
        public override PseudoinstructionType Type => PseudoinstructionType.Jl;

        protected override IEnumerable<IEnumerable<BitArray>> ParseInner(TokenReader reader) {
            ParseParameters(reader, out var parsedTokens, TokenClass.Identifier);
            throw new LabelNotResolvedException((IdentifierToken)parsedTokens[0], SizeInBytes, ParseLabel);
        }

        private BitArray[] ParseLabel(ushort labelAddress) {
            return ParseLabelInner(labelAddress).SelectMany(bitArray => bitArray).ToArray();
        }

        private IEnumerable<IEnumerable<BitArray>> ParseLabelInner(ushort labelAddress) {
            yield return InstructionEncoder.Encode(McInstructionType.SetI, Regs.Ass, (byte)(labelAddress & 0x00FF));
            yield return InstructionEncoder.Encode(McInstructionType.SethI, Regs.Ass, (byte)(labelAddress & 0xFF00));
            yield return InstructionEncoder.Encode(McInstructionType.Jr, Regs.Zero, Regs.Zero, Regs.Ass);
        }
    }
}

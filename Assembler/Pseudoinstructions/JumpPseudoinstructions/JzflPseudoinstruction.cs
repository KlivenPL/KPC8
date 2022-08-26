using Assembler._Infrastructure;
using Assembler.Readers;
using Assembler.Tokens;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assembler.Pseudoinstructions.JumpPseudoinstructions {
    /// <summary>
    /// Jl #label
    /// </summary>
    class JzflPseudoinstruction : PseudoinstructionBase {
        private const int SizeInBytes = 3 * 2;
        public override PseudoinstructionType Type => PseudoinstructionType.Jzfl;

        protected override IEnumerable<IEnumerable<BitArray>> ParseInner(TokenReader reader) {
            ParseParameters<IdentifierToken>(reader, out var identifierToken);
            throw new LabelNotResolvedException(identifierToken, labelsContext.CurrentRegion, SizeInBytes, ParseLabel);
        }

        private BitArray[] ParseLabel(ushort labelAddress) {
            return ParseLabelInner(labelAddress).SelectMany(bitArray => bitArray).ToArray();
        }

        private IEnumerable<IEnumerable<BitArray>> ParseLabelInner(ushort labelAddress) {
            SplitWord(labelAddress, out var lower, out var higher);

            yield return InstructionEncoder.Encode(McInstructionType.SetI, Regs.Ass, lower);
            yield return InstructionEncoder.Encode(McInstructionType.SethI, Regs.Ass, higher);
            yield return InstructionEncoder.Encode(McInstructionType.Jzf, Regs.Zero, Regs.Zero, Regs.Ass);
        }
    }
}

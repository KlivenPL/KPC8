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
    class JaslPseudoinstruction : PseudoinstructionBase {
        private const int SizeInBytes = 3 * 2;
        public override PseudoinstructionType Type => PseudoinstructionType.Jasl;

        protected override IEnumerable<IEnumerable<BitArray>> ParseInner(TokenReader reader) {
            ParseParameters<IdentifierToken, RegisterToken>(reader, out var identifierToken, out var saveRegisterToken);
            throw new LabelNotResolvedException(identifierToken, labelsContext.CurrentRegion, SizeInBytes, addr => ParseLabel(addr, saveRegisterToken));
        }

        private BitArray[] ParseLabel(ushort labelAddress, RegisterToken saveRegisterToken) {
            return ParseLabelInner(labelAddress, saveRegisterToken).SelectMany(bitArray => bitArray).ToArray();
        }

        private IEnumerable<IEnumerable<BitArray>> ParseLabelInner(ushort labelAddress, RegisterToken saveRegisterToken) {
            SplitWord(labelAddress, out var lower, out var higher);

            yield return InstructionEncoder.Encode(McInstructionType.SetI, Regs.Ass, lower);
            yield return InstructionEncoder.Encode(McInstructionType.SethI, Regs.Ass, higher);
            yield return InstructionEncoder.Encode(McInstructionType.Jas, Regs.Zero, Regs.Ass, saveRegisterToken.Value);
        }
    }
}

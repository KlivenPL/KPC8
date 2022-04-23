using Assembler.Builders;
using Assembler.Contexts;
using Assembler.Readers;
using Assembler.Tokens;

namespace Assembler.Commands {
    internal class SetAddressCommand : CommandBase {
        public override CommandType Type => CommandType.SetAddress;

        protected override string[] AcceptedRegions => new[] { LabelsContext.ConstRegion, LabelsContext.CodeRegion };

        protected override void ParseInner(TokenReader reader, LabelsContext labelsContext, RomBuilder romBuilder) {
            ParseParameters<NumberToken>(reader, out var addressToJumpToken);
            romBuilder.NextAddress = addressToJumpToken.Value;
        }
    }
}

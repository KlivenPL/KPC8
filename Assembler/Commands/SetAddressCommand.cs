using Assembler.Builders;
using Assembler.Contexts.Labels;
using Assembler.Readers;
using Assembler.Tokens;

namespace Assembler.Commands {
    internal class SetAddressCommand : CommandBase {
        public override CommandType Type => CommandType.SetAddress;

        protected override CommandAllowedIn AcceptedRegions => CommandAllowedIn.ConstRegion | CommandAllowedIn.UserDefinedRegion;

        protected override void ParseInner(TokenReader reader, LabelsContext labelsContext, RomBuilder romBuilder) {
            ParseParameters<NumberToken>(reader, out var addressToJumpToken);
            romBuilder.NextAddress = addressToJumpToken.Value;
        }
    }
}

using Assembler.Builders;
using Assembler.Contexts.Labels;
using Assembler.Contexts.Regions;
using Assembler.Readers;
using Assembler.Tokens;

namespace Assembler.Commands {
    internal class SetModuleAddressCommand : CommandBase {
        public override CommandType Type => CommandType.SetModuleAddress;

        protected override CommandAllowedIn AcceptedRegions => CommandAllowedIn.ConstRegion;

        protected override void PreParseInner(TokenReader reader, IRegion region) {
            ParseParameters<NumberToken>(reader, out var addressToJumpToken);

            if (region is ConstRegion constRegion) {
                constRegion.SetNextModuleAddress(addressToJumpToken.Value);
            }
        }

        protected override void ParseInner(TokenReader reader, LabelsContext labelsContext, RomBuilder romBuilder) {
            ParseParameters<NumberToken>(reader, out var _);
        }
    }
}

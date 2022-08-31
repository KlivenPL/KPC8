using Assembler.Builders;
using Assembler.Contexts.Labels;
using Assembler.Contexts.Regions;
using Assembler.Readers;
using Assembler.Tokens;

namespace Assembler.Commands {
    internal class ExportRegionCommand : CommandBase {
        public override CommandType Type => CommandType.ExportRegion;

        protected override CommandAllowedIn AcceptedRegions => CommandAllowedIn.ModuleRegion;

        protected override void PreParseInner(TokenReader reader, IRegion region) {
            ParseParameters<IdentifierToken>(reader, out var identifierToken);

            var module = (ModuleRegion)region;
            module.AddAwaitingForExportRegionName(identifierToken.Value);
        }

        protected override void ParseInner(TokenReader reader, LabelsContext labelsContext, RomBuilder romBuilder) {
            ParseParameters<IdentifierToken>(reader, out var _);
        }
    }
}

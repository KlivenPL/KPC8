using Assembler._Infrastructure;
using Assembler.Builders;
using Assembler.Contexts;
using Assembler.Readers;
using Assembler.Tokens;

namespace Assembler.Commands {
    internal class DefnumCommand : CommandBase {
        public override CommandType Type => CommandType.Defnum;

        protected override string[] AcceptedRegions => new[] { LabelsContext.CodeRegion };

        protected override void ParseInner(TokenReader reader, LabelsContext labelsContext, RomBuilder romBuilder) {
            ParseParameters<IdentifierToken, NumberToken>(reader, out var identifierToken, out var numToken);

            if (!labelsContext.TryInsertRegionedToken(identifierToken.Value, numToken, out var errorMessage)) {
                throw ParserException.Create(errorMessage, reader.Current);
            }
        }
    }
}

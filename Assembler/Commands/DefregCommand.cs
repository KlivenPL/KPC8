using Assembler._Infrastructure;
using Assembler.Builders;
using Assembler.Contexts;
using Assembler.Readers;
using Assembler.Tokens;

namespace Assembler.Commands {
    internal class DefregCommand : CommandBase {
        public override CommandType Type => CommandType.Defreg;

        protected override string[] AcceptedRegions => new[] { LabelsContext.CodeRegion };

        protected override void ParseInner(TokenReader reader, LabelsContext labelsContext, RomBuilder romBuilder) {
            ParseParameters<IdentifierToken, RegisterToken>(reader, out var identifierToken, out var regToken);

            var regTokenCopy = new RegisterToken(regToken.Value, regToken.CodePosition, regToken.LineNumber);

            if (!labelsContext.TryInsertRegionedToken(identifierToken.Value, regTokenCopy, out var errorMessage)) {
                throw ParserException.Create(errorMessage, reader.Current);
            }
        }
    }
}

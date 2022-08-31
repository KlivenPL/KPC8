using Assembler._Infrastructure;
using Assembler.Builders;
using Assembler.Contexts.Labels;
using Assembler.Contexts.Regions;
using Assembler.Readers;
using Assembler.Tokens;

namespace Assembler.Commands {
    internal class DefnumCommand : CommandBase {
        public override CommandType Type => CommandType.Defnum;

        protected override CommandAllowedIn AcceptedRegions => CommandAllowedIn.UserDefinedRegion;

        protected override void PreParseInner(TokenReader reader, IRegion region) {
            ParseParameters<IdentifierToken, NumberToken>(reader, out var identifierToken, out var numToken);

            var udr = (UserDefinedRegion)region;
            if (udr.IsExported) {
                Defnum(reader, identifierToken, numToken);
            }
        }

        protected override void ParseInner(TokenReader reader, LabelsContext labelsContext, RomBuilder romBuilder) {
            ParseParameters<IdentifierToken, NumberToken>(reader, out var identifierToken, out var numToken);

            var udr = (UserDefinedRegion)labelsContext.CurrentRegion;
            if (!udr.IsExported) {
                Defnum(reader, identifierToken, numToken);
            }
        }

        private void Defnum(TokenReader reader, IdentifierToken identifierToken, NumberToken numToken) {
            var numTokenCopy = new NumberToken(numToken.Value, numToken.CodePosition, numToken.LineNumber, identifierToken.FilePath);

            if (!TryInsertToken(identifierToken.Value, numTokenCopy, out var errorMessage)) {
                throw ParserException.Create(errorMessage, reader.Current);
            }

            AddConstantDebugSymbol?.Invoke(new DebugData.ConstantValueSymbol(identifierToken.LineNumber, identifierToken.Value, numToken.Value.ToString(), false));
        }
    }
}

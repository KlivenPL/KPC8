using Assembler._Infrastructure;
using Assembler.Builders;
using Assembler.Contexts.Labels;
using Assembler.Contexts.Regions;
using Assembler.Readers;
using Assembler.Tokens;

namespace Assembler.Commands {
    internal class DefregCommand : CommandBase {
        public override CommandType Type => CommandType.Defreg;

        protected override CommandAllowedIn AcceptedRegions => CommandAllowedIn.UserDefinedRegion;

        protected override void PreParseInner(TokenReader reader, IRegion region) {
            ParseParameters<IdentifierToken, RegisterToken>(reader, out var identifierToken, out var regToken);

            var udr = (UserDefinedRegion)region;
            if (udr.IsExported) {
                Defreg(reader, identifierToken, regToken);
            }
        }

        protected override void ParseInner(TokenReader reader, LabelsContext labelsContext, RomBuilder romBuilder) {
            ParseParameters<IdentifierToken, RegisterToken>(reader, out var identifierToken, out var regToken);

            var udr = (UserDefinedRegion)labelsContext.CurrentRegion;
            if (!udr.IsExported) {
                Defreg(reader, identifierToken, regToken);
            }
        }

        private void Defreg(TokenReader reader, IdentifierToken identifierToken, RegisterToken regToken) {
            var regTokenCopy = new RegisterToken(regToken.Value, regToken.CodePosition, regToken.LineNumber, identifierToken.FilePath);

            if (!TryInsertToken(identifierToken.Value, regTokenCopy, out var errorMessage)) {
                throw ParserException.Create(errorMessage, reader.Current);
            }

            AddConstantDebugSymbol?.Invoke(new DebugData.ConstantValueSymbol(identifierToken.LineNumber, identifierToken.Value, regToken.Value.ToString(), true));
        }
    }
}

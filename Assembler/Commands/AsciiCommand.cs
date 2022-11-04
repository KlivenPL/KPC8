using Assembler._Infrastructure;
using Assembler.Builders;
using Assembler.Contexts.Labels;
using Assembler.Readers;
using Assembler.Tokens;
using Infrastructure.BitArrays;
using System.Text;

namespace Assembler.Commands {
    internal class AsciiCommand : CommandBase {
        public override CommandType Type => CommandType.Ascii;

        protected override CommandAllowedIn AcceptedRegions => CommandAllowedIn.ConstRegion;

        protected override void ParseInner(TokenReader reader, LabelsContext labelsContext, RomBuilder romBuilder) {
            ParseParameters<IdentifierToken, StringToken>(reader, out var identifierToken, out var strToken);

            var numberToken = new NumberToken(romBuilder.NextAddress, identifierToken.CodePosition, identifierToken.LineNumber, identifierToken.FilePath);

            if (!TryInsertToken(identifierToken.Value, numberToken, out var errorMessage)) {
                throw ParserException.Create(errorMessage, reader.Current);
            }

            var strToStore = $"{strToken.Value}";
            var strBytes = Encoding.ASCII.GetBytes(strToStore);
            for (int i = 0; i < strBytes.Length; i++) {
                romBuilder.AddByte(BitArrayHelper.FromByteLE(strBytes[i]));
            }
        }
    }
}

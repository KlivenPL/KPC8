using Assembler._Infrastructure;
using Assembler.Builders;
using Assembler.Contexts;
using Assembler.Readers;
using Assembler.Tokens;

namespace Assembler.Commands {
    internal class DefcolorRGBCommand : CommandBase {
        public override CommandType Type => CommandType.DefcolorRGB;

        protected override string[] AcceptedRegions => new[] { LabelsContext.CodeRegion, LabelsContext.ConstRegion };

        protected override void ParseInner(TokenReader reader, LabelsContext labelsContext, RomBuilder romBuilder) {
            ParseParameters<IdentifierToken, NumberToken, NumberToken, NumberToken>(reader, out var identifierToken, out var r8Token, out var g8Token, out var b8Token);

            if (r8Token.Value > 255 || g8Token.Value > 255 || b8Token.Value > 255 || r8Token.Value < 0 || g8Token.Value < 0 || b8Token.Value < 0) {
                throw ParserException.Create("RGB Color must be in range [0, 255]. Then it is converted to 15-bit space.", identifierToken);
            }

            byte r5b = (byte)(r8Token.Value / 8);
            byte g5b = (byte)(g8Token.Value / 8);
            byte b5b = (byte)(b8Token.Value / 8);

            byte b1 = (byte)((r5b << 2) | ((g5b & 0b00011000) >> 3));
            byte b2 = (byte)(((g5b & 0b00000111) << 5) | b5b);

            ushort rgb15 = (ushort)(b1 << 8 | b2);

            var rgb15Token = new NumberToken(rgb15, identifierToken.CodePosition, identifierToken.LineNumber);

            if (!labelsContext.TryInsertRegionedToken(identifierToken.Value, rgb15Token, out var errorMessage)) {
                throw ParserException.Create(errorMessage, reader.Current);
            }
        }
    }
}

using Assembler._Infrastructure;
using Assembler.Builders;
using Assembler.Contexts.Labels;
using Assembler.Contexts.Regions;
using Assembler.Readers;
using Assembler.Tokens;

namespace Assembler.Commands {
    internal class DefcolorRGBCommand : CommandBase {
        public override CommandType Type => CommandType.DefcolorRGB;

        protected override CommandAllowedIn AcceptedRegions => CommandAllowedIn.ConstRegion | CommandAllowedIn.UserDefinedRegion;

        protected override void PreParseInner(TokenReader reader, IRegion region) {
            ParseParameters<IdentifierToken, NumberToken, NumberToken, NumberToken>(reader, out var identifierToken, out var r8Token, out var g8Token, out var b8Token);

            var udr = (UserDefinedRegion)region;
            if (udr.IsExported) {
                DefcolorRGB(reader, identifierToken, r8Token, g8Token, b8Token);
            }
        }

        protected override void ParseInner(TokenReader reader, LabelsContext labelsContext, RomBuilder romBuilder) {
            ParseParameters<IdentifierToken, NumberToken, NumberToken, NumberToken>(reader, out var identifierToken, out var r8Token, out var g8Token, out var b8Token);

            var udr = (UserDefinedRegion)labelsContext.CurrentRegion;
            if (!udr.IsExported) {
                DefcolorRGB(reader, identifierToken, r8Token, g8Token, b8Token);
            } else {
                byte r5b = (byte)(r8Token.Value / 8);
                byte g5b = (byte)(g8Token.Value / 8);
                byte b5b = (byte)(b8Token.Value / 8);

                AddDebugSymbol(identifierToken, r5b, g5b, b5b);
            }
        }

        private void DefcolorRGB(TokenReader reader, IdentifierToken identifierToken, NumberToken r8Token, NumberToken g8Token, NumberToken b8Token) {
            if (r8Token.Value > 255 || g8Token.Value > 255 || b8Token.Value > 255 || r8Token.Value < 0 || g8Token.Value < 0 || b8Token.Value < 0) {
                throw ParserException.Create("RGB Color must be in range [0, 255]. Then it is converted to 15-bit space.", identifierToken);
            }

            byte r5b = (byte)(r8Token.Value / 8);
            byte g5b = (byte)(g8Token.Value / 8);
            byte b5b = (byte)(b8Token.Value / 8);

            byte b1 = (byte)((r5b << 2) | ((g5b & 0b00011000) >> 3));
            byte b2 = (byte)(((g5b & 0b00000111) << 5) | b5b);

            ushort rgb15 = (ushort)(b1 << 8 | b2);

            var rgb15Token = new NumberToken(rgb15, identifierToken.CodePosition, identifierToken.LineNumber, identifierToken.FilePath);

            if (!TryInsertToken(identifierToken.Value, rgb15Token, out var errorMessage)) {
                throw ParserException.Create(errorMessage, reader.Current);
            }

            AddDebugSymbol(identifierToken, r5b, g5b, b5b);
        }

        private void AddDebugSymbol(IdentifierToken identifierToken, byte r5b, byte g5b, byte b5b) {
            AddConstantDebugSymbol?.Invoke(new DebugData.ConstantValueSymbol(identifierToken.FilePath, identifierToken.LineNumber, identifierToken.Value, $"R:{r5b} G:{g5b} B:{b5b}", false));
        }
    }
}

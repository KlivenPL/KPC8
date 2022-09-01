using Assembler._Infrastructure;
using Assembler.Builders;
using Assembler.Contexts.Labels;
using Assembler.Contexts.Regions;
using Assembler.Readers;
using Assembler.Tokens;

namespace Assembler.Commands {
    internal class DefcolorHEXCommand : CommandBase {
        public override CommandType Type => CommandType.DefcolorHEX;

        protected override CommandAllowedIn AcceptedRegions => CommandAllowedIn.ConstRegion | CommandAllowedIn.UserDefinedRegion;

        protected override void PreParseInner(TokenReader reader, IRegion region) {
            ParseParameters<IdentifierToken, StringToken>(reader, out var identifierToken, out var hexColorToken);

            var udr = (UserDefinedRegion)region;
            if (udr.IsExported) {
                DefcolorHEX(reader, identifierToken, hexColorToken);
            }
        }

        protected override void ParseInner(TokenReader reader, LabelsContext labelsContext, RomBuilder romBuilder) {
            ParseParameters<IdentifierToken, StringToken>(reader, out var identifierToken, out var hexColorToken);

            var udr = (UserDefinedRegion)labelsContext.CurrentRegion;
            if (!udr.IsExported) {
                DefcolorHEX(reader, identifierToken, hexColorToken);
            } else {
                if (!byte.TryParse(hexColorToken.Value.Substring(1, 2), System.Globalization.NumberStyles.HexNumber, null, out var r8b)) {
                    throw ParserException.Create("HEX Color must be in format #XXXXXX. Then it is converted to 15-bit space.", identifierToken);
                }

                if (!byte.TryParse(hexColorToken.Value.Substring(3, 2), System.Globalization.NumberStyles.HexNumber, null, out var g8b)) {
                    throw ParserException.Create("HEX Color must be in format #XXXXXX. Then it is converted to 15-bit space.", identifierToken);
                }

                if (!byte.TryParse(hexColorToken.Value.Substring(5, 2), System.Globalization.NumberStyles.HexNumber, null, out var b8b)) {
                    throw ParserException.Create("HEX Color must be in format #XXXXXX. Then it is converted to 15-bit space.", identifierToken);
                }

                byte r5b = (byte)(r8b / 8);
                byte g5b = (byte)(g8b / 8);
                byte b5b = (byte)(b8b / 8);
                AddDebugSymbol(identifierToken, r5b, g5b, b5b);
            }
        }

        private void DefcolorHEX(TokenReader reader, IdentifierToken identifierToken, StringToken hexColorToken) {
            if (hexColorToken.Value.Length != 7 || !hexColorToken.Value.StartsWith('#')) {
                throw ParserException.Create("HEX Color must be in format #XXXXXX. Then it is converted to 15-bit space.", identifierToken);
            }

            if (!byte.TryParse(hexColorToken.Value.Substring(1, 2), System.Globalization.NumberStyles.HexNumber, null, out var r8b)) {
                throw ParserException.Create("HEX Color must be in format #XXXXXX. Then it is converted to 15-bit space.", identifierToken);
            }

            if (!byte.TryParse(hexColorToken.Value.Substring(3, 2), System.Globalization.NumberStyles.HexNumber, null, out var g8b)) {
                throw ParserException.Create("HEX Color must be in format #XXXXXX. Then it is converted to 15-bit space.", identifierToken);
            }

            if (!byte.TryParse(hexColorToken.Value.Substring(5, 2), System.Globalization.NumberStyles.HexNumber, null, out var b8b)) {
                throw ParserException.Create("HEX Color must be in format #XXXXXX. Then it is converted to 15-bit space.", identifierToken);
            }

            byte r5b = (byte)(r8b / 8);
            byte g5b = (byte)(g8b / 8);
            byte b5b = (byte)(b8b / 8);

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

using Assembler.Readers;

namespace Assembler.Tokens {
    class CharToken : TokenBase<char> {
        public override char Value { get; protected set; }
        public override TokenClass Class => TokenClass.Char;

        public override bool TryAccept(CodeReader reader) {
            if (reader.Current == '\'') {
                if (reader.EndOfCode) {
                    return false;
                }

                if (reader.Read() && reader.Current != '\'') {
                    if (reader.EndOfCode) {
                        return false;
                    }

                    var append = reader.Current;
                    if (reader.Current == '\\') {
                        if (reader.Read() && TryParseSpecialCharacter(reader.Current, out var specialChar)) {
                            append = specialChar;
                        } else {
                            return false;
                        }
                    }

                    if (reader.Read() && reader.Current == '\'') {
                        Value = append;
                        return true;
                    }
                }
            }

            return false;
        }

        private bool TryParseSpecialCharacter(char c, out char result) {
            result = c switch {
                '\'' => '\'',
                '\\' => '\\',
                'n' => '\n',
                't' => '\t',
                '0' => '\0',
                _ => '\r',
            };

            return result != '\r';
        }
    }
}

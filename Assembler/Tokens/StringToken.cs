using Assembler.Readers;
using System.Text;

namespace Assembler.Tokens {
    class StringToken : TokenBase<string> {
        public override string Value { get; protected set; }
        public override TokenClass Class => TokenClass.String;

        public StringToken() {

        }

        public StringToken(string value, int position, int line) {
            Value = value;
            AddDebugData(position, line);
        }

        public override IToken DeepCopy() {
            return new StringToken(Value, CodePosition, LineNumber);
        }

        public override bool TryAccept(CodeReader reader) {
            if (reader.Current == '"') {
                var sb = new StringBuilder();

                if (reader.EndOfCode) {
                    return false;
                }

                while (reader.Read() && reader.Current != '"') {
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

                    sb.Append(append);
                }

                Value = sb.ToString();
                return true;
            }

            return false;
        }

        private bool TryParseSpecialCharacter(char c, out char result) {
            result = c switch {
                '"' => '"',
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

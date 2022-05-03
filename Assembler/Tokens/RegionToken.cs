using Assembler.Readers;
using System.Text;

namespace Assembler.Tokens {
    class RegionToken : TokenBase<string> {
        public override string Value { get; protected set; }
        public override TokenClass Class => TokenClass.Region;

        public RegionToken() {

        }

        public RegionToken(string value, int position, int line) {
            Value = value;
            AddDebugData(position, line);
        }

        public override IToken DeepCopy() {
            return new RegionToken(Value, CodePosition, LineNumber);
        }

        public override bool TryAccept(CodeReader reader) {
            if (reader.Current == '*') {
                var sb = new StringBuilder();

                while (reader.Read() && (char.IsLetterOrDigit(reader.Current) || reader.Current == '_' || reader.Current == '@')) {
                    sb.Append(reader.Current);
                }

                if (sb.Length > 0) {
                    Value = sb.ToString();
                    return true;
                }
            }
            return false;
        }
    }
}

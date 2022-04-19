using Assembler.Readers;
using System.Text;

namespace Assembler.Tokens {
    class NumberToken : TokenBase<ushort> {
        public NumberToken() {

        }

        public NumberToken(ushort value, int position, int line) {
            Value = value;
            AddDebugData(position, line);
        }

        public override ushort Value { get; protected set; }
        public override TokenClass Class => TokenClass.Number;

        public override bool TryAccept(CodeReader reader) {
            if (char.IsDigit(reader.Current) || reader.Current == '-') {
                var sb = new StringBuilder(reader.Current.ToString());

                while (reader.Read() && (char.IsDigit(reader.Current) || IsCharacterAccepted(reader.LowerCurrent))) {
                    sb.Append(reader.Current);
                }

                var input = sb.ToString().ToLower();
                if (input[0] == '-') {
                    if (short.TryParse(input, out var signedResult)) {
                        Value = (ushort)signedResult;
                        return true;
                    }
                }

                if (input.StartsWith("0x")) {
                    if (ushort.TryParse(input[2..], System.Globalization.NumberStyles.AllowHexSpecifier, null, out var unsignedHexResult)) {
                        Value = unsignedHexResult;
                        return true;
                    }
                }

                if (input.StartsWith("0b")) {
                    if (TryBinToDec(input[2..], out var binaryResult)) {
                        Value = binaryResult;
                        return true;
                    }
                }

                if (ushort.TryParse(input, out var unsignedResult)) {
                    Value = unsignedResult;
                    return true;
                }
            }
            return false;
        }

        private bool IsCharacterAccepted(char c) {
            return c switch {
                'a' => true,
                'b' => true,
                'c' => true,
                'd' => true,
                'e' => true,
                'f' => true,
                'x' => true,
                _ => false,
            };
        }

        private bool TryBinToDec(string val, out ushort result) {
            result = 0;

            if (val.Length == 0)
                return false;

            var tmpSum = 0;

            for (int i = val.Length - 1; i >= 0; i--) {
                if (val[i] == '0' || val[i] == '1') {
                    var numVal = int.Parse(val[i].ToString());
                    tmpSum += numVal == 1 ? 1 << val.Length - i - 1 : 0;
                } else {
                    return false;
                }
            }

            if (tmpSum >= 0 && tmpSum <= ushort.MaxValue) {
                result = (ushort)tmpSum;
                return true;
            }

            return false;
        }
    }
}

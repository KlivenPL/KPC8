using Assembler.Pseudoinstructions;
using Assembler.Readers;
using KPC8.RomProgrammers.Microcode;
using System;
using System.Text;

namespace Assembler.Tokens {
    class IdentifierToken : TokenBase<string> {
        public override string Value { get; protected set; }
        public override TokenClass Class => TokenClass.Identifier;

        public IdentifierToken() { }

        public IdentifierToken(string value, int position, int line, string filePath) {
            Value = value;
            AddDebugData(position, line, filePath);
        }

        public override IToken DeepCopy() {
            return new IdentifierToken(Value, CodePosition, LineNumber, FilePath);
        }

        public override bool TryAccept(CodeReader reader) {
            if (char.IsLetter(reader.Current) || reader.Current == '@') {
                var sb = new StringBuilder(reader.Current.ToString());

                while (reader.Read() && (char.IsLetterOrDigit(reader.Current) || reader.Current == '_' || reader.Current == '.')) {
                    sb.Append(reader.Current);
                }

                Value = sb.ToString();
                return true;
            }
            return false;
        }

        public bool IsInstruction(out McInstructionType instructionType) {
            if (Enum.TryParse(Value, true, out instructionType)) {
                return true;
            }
            return false;
        }

        public bool IsPseudoinstruction(out PseudoinstructionType pseudoinstructionType) {
            if (Enum.TryParse(Value, true, out pseudoinstructionType)) {
                return true;
            }
            return false;
        }
    }
}

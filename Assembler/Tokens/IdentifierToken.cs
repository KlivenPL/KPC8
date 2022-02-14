using Assembler.Readers;
using KPC8.RomProgrammers.Microcode;
using System;
using System.Text;

namespace Assembler.Tokens {
    class IdentifierToken : TokenBase<string> {
        public override string Value { get; protected set; }
        public override TokenClass Class => TokenClass.Identifier;

        public override bool TryAccept(CodeReader reader) {
            if (char.IsLetter(reader.Current)) {
                var sb = new StringBuilder(reader.Current.ToString());

                while (reader.Read() && (char.IsLetterOrDigit(reader.Current) || reader.Current == '_')) {
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
    }
}

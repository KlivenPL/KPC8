using Assembler.Readers;
using KPC8.ProgRegs;
using System;
using System.Text;

namespace Assembler.Tokens {
    class RegisterToken : TokenBase<Regs> {
        public override Regs Value { get; protected set; }
        public override TokenClass Class => TokenClass.Register;

        public RegisterToken() {

        }

        public RegisterToken(Regs value, int position, int line) {
            Value = value;
            AddDebugData(position, line);
        }

        public override IToken DeepCopy() {
            return new RegisterToken(Value, CodePosition, LineNumber);
        }

        public override bool TryAccept(CodeReader reader) {
            if (reader.Current == '$') {
                var sb = new StringBuilder();

                while (reader.Read() && char.IsLetterOrDigit(reader.Current)) {
                    sb.Append(reader.Current);
                }

                if (Enum.TryParse<Regs>(sb.ToString(), true, out var register)) {
                    if (register != Regs.None) {
                        Value = register;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

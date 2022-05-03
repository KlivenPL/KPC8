using Assembler.Commands;
using Assembler.Readers;
using System;
using System.Text;

namespace Assembler.Tokens {
    class CommandToken : TokenBase<CommandType> {
        public override CommandType Value { get; protected set; }
        public override TokenClass Class => TokenClass.Command;

        public CommandToken() {

        }

        public CommandToken(CommandType value, int position, int line) {
            Value = value;
            AddDebugData(position, line);
        }

        public override IToken DeepCopy() {
            return new CommandToken(Value, CodePosition, LineNumber);
        }

        public override bool TryAccept(CodeReader reader) {
            if (reader.Current == '.') {
                var sb = new StringBuilder();

                while (reader.Read() && char.IsLetterOrDigit(reader.Current)) {
                    sb.Append(reader.Current);
                }

                if (Enum.TryParse<CommandType>(sb.ToString(), true, out var command)) {
                    if (command != CommandType.None) {
                        Value = command;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

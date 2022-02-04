using Assembler.Readers;
using KPC8.RomProgrammers.Microcode;
using System;
using System.Text;

namespace Assembler.Tokens {
    class OperationToken : TokenBase<McInstructionType> {
        public override McInstructionType Value { get; protected set; }
        public override TokenClass Class => TokenClass.Operation;

        public override bool TryAccept(CodeReader reader) {
            if (char.IsLetter(reader.Current)) {
                var sb = new StringBuilder(reader.Current.ToString());

                while (reader.Read() && char.IsLetter(reader.Current)) {
                    sb.Append(reader.Current);
                }

                if (Enum.TryParse<McInstructionType>(sb.ToString(), true, out var command)) {
                    Value = command;
                    return true;
                }
            }
            return false;
        }
    }
}

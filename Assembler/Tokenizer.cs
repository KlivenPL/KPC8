using Assembler._Infrastructure;
using Assembler.Readers;
using Assembler.Tokens;
using System.Collections.Generic;

namespace Assembler {
    public class Tokenizer {
        public IEnumerable<IToken> Tokenize(CodeReader reader) {
            while (reader.Read()) {
                if (char.IsWhiteSpace(reader.Current) || reader.Current == ',') {
                    continue;
                }

                if (reader.Current == '/' && reader.TryPeek(out var c) && c == '/') {
                    reader.SkipToNextLine();
                    continue;
                }

                var token = CreateToken(reader);

                if (token.TryAccept(reader)) {
                    yield return token;
                } else {
                    throw TokenizerException.Create($"Could not tokenize value as {token.Class}", reader);
                }
            }
        }

        private IToken CreateToken(CodeReader reader) {
            if (char.IsDigit(reader.Current) || reader.Current == '-') {
                return new NumberToken();
            } else if (char.IsLetter(reader.Current)) {
                return new OperationToken();
            }

            return reader.Current switch {
                '$' => new RegisterToken(),
                ':' => new LabelToken(),
                '*' => new RegionToken(),
                '.' => new CommandToken(),
                '\'' => new CharToken(),
                '"' => new StringToken(),
                _ => throw TokenizerException.Create($"Unexpected character: {reader.Current}", reader)
            };
        }
    }
}

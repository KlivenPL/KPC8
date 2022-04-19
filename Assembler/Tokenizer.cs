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
            var position = reader.Position;
            var line = reader.Line;

            if (char.IsDigit(reader.Current) || reader.Current == '-') {
                return new NumberToken().AddDebugData(position, line);
            } else if (char.IsLetter(reader.Current) || reader.Current == '@') {
                return new IdentifierToken().AddDebugData(position, line);
            }

            return reader.Current switch {
                '$' => new RegisterToken().AddDebugData(position, line),
                ':' => new LabelToken().AddDebugData(position, line),
                '*' => new RegionToken().AddDebugData(position, line),
                '.' => new CommandToken().AddDebugData(position, line),
                '\'' => new CharToken().AddDebugData(position, line),
                '"' => new StringToken().AddDebugData(position, line),
                _ => throw TokenizerException.Create($"Unexpected character: {reader.Current}", reader)
            };
        }
    }
}

using Assembler.Contexts;
using Assembler.Readers;

namespace Assembler {
    public class Parser {

        private LabelsContext labels;
        private TokenReader reader;

        public Parser(TokenReader reader) {
            this.reader = reader;
            labels = new LabelsContext();
        }

        /*        public BitArray[] Parse() {

                    if (!labels.TryParseAllRegionsAndLabels(reader.Clone())) {
                        throw ParserException.Create("Error while parsing labels", reader.Current);
                    }

                    while (reader.Read()) {

                    }
                }*/
        /*
                private IToken CreateToken(CodeReader reader) {
                    var position = reader.Position;
                    var line = reader.Line;

                    if (char.IsDigit(reader.Current) || reader.Current == '-') {
                        return new NumberToken().AddDebugData(position, line);
                    } else if (char.IsLetter(reader.Current)) {
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
                }*/
    }
}

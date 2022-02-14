using Assembler.Tokens;
using System;
using System.Text;

namespace Assembler._Infrastructure {
    class ParserException : Exception {
        public int Position { get; }
        public int Line { get; }
        public string LineText { get; }

        public static ParserException Create(string message, IToken token) {
            var sb = new StringBuilder();
            sb.AppendLine(message);
            sb.AppendLine(CreateExceptionDetails(token));
            return new ParserException(sb.ToString(), token.CodePosition, token.LineNumber, token.ToString());
        }

        private ParserException(string message, int position, int line, string lineText) : base(message) {
            Position = position;
            Line = line;
            LineText = lineText;
        }

        private static string CreateExceptionDetails(IToken token) {
            return $"At line {token.LineNumber}, pos {token.CodePosition}, near ...{token}...";
        }
    }
}

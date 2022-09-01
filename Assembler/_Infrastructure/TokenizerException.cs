using _Infrastructure.Strings;
using Assembler.Readers;
using System;
using System.Text;

namespace Assembler._Infrastructure {
    public class TokenizerException : Exception {
        public int Position { get; }
        public int Line { get; }
        public string LineText { get; }

        public static TokenizerException Create(string message, CodeReader codeReader) {
            var sb = new StringBuilder();
            sb.AppendLine("Tokenizer exception:");
            sb.AppendLine(message);
            sb.AppendLine(CreateExceptionDetails(codeReader));
            return new TokenizerException(sb.ToString(), codeReader.Position, codeReader.Line, codeReader.LineText);
        }

        private TokenizerException(string message, int position, int line, string lineText) : base(message) {
            Position = position;
            Line = line;
            LineText = lineText;
        }

        private static string CreateExceptionDetails(CodeReader codeReader) {
            return $"In file {codeReader.FilePath}:{codeReader.Line}, pos {codeReader.Position}, near ...{codeReader.LineText.Right(30)}...";
        }
    }
}

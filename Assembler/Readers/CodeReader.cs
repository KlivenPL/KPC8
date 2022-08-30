using System;
using System.IO;
using System.Text;

namespace Assembler.Readers {
    public sealed class CodeReader : IDisposable {
        private readonly StreamReader reader;

        private readonly StringBuilder lastLineBuilder;
        public string LineText => lastLineBuilder.ToString();

        public bool EndOfCode => reader.EndOfStream;
        public char Current { get; private set; }
        public char LowerCurrent => char.ToLower(Current);

        public int Position { get; private set; }
        public int Line { get; private set; } = 1;
        public string FilePath { get; }

        public CodeReader(MemoryStream ms) {
            reader = new StreamReader(ms);
            lastLineBuilder = new StringBuilder();
        }

        public CodeReader(MemoryStream ms, string filePath) : this(ms) {
            FilePath = filePath;
        }

        public bool Read() {
            if (!reader.EndOfStream) {
                Current = (char)reader.Read();
                Position++;
                lastLineBuilder.Append(Current);

                if (Current == '\n') {
                    Line++;
                    Position = 0;
                    lastLineBuilder.Clear();
                }

                return true;
            }
            return false;
        }

        public bool TryPeek(out char c) {
            c = (char)0;
            var tmp = reader.Peek();
            if (tmp == -1)
                return false;
            c = (char)tmp;
            return true;
        }

        public void SkipToNextLine() {
            while (Read()) {
                if (Current == '\n') {
                    return;
                }
            }
        }

        public void Dispose() {
            reader.Dispose();
        }
    }
}

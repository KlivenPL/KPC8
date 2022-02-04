using System;
using System.Collections.Generic;
using System.IO;

namespace Assembler.Readers {
    public sealed class CodeReader : IDisposable {
        private readonly StreamReader reader;
        private readonly MemoryStream ms;
        private readonly Stack<long> checkpoints;

        public bool EndOfCode => reader.EndOfStream;
        public char Current { get; private set; }
        public char LowerCurrent => char.ToLower(Current);

        public CodeReader(MemoryStream ms) {
            this.ms = ms;
            reader = new StreamReader(ms);
            checkpoints = new Stack<long>();
        }

        public bool Read() {
            if (!reader.EndOfStream) {
                Current = (char)reader.Read();
                return true;
            }
            return false;
        }

        public bool Peek(out char c) {
            c = (char)0;
            var tmp = reader.Peek();
            if (tmp == -1)
                return false;
            c = (char)tmp;
            return true;
        }

        public void CreateCheckpoint() {
            checkpoints.Push(ms.Position);
        }

        public void GoBack() {
            if (checkpoints.Count == 0) {
                throw new Exception("No checkpoints to go back");
            }
            ms.Position = checkpoints.Pop();
        }

        public void Dispose() {
            reader.Dispose();
        }
    }
}

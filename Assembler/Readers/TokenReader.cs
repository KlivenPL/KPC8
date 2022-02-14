using Assembler.Tokens;
using System.Collections.Generic;
using System.Linq;

namespace Assembler.Readers {
    public sealed class TokenReader {
        private readonly IToken[] tokens;
        private readonly int size;

        public IToken Current { get; private set; }
        public TToken CastCurrent<TToken>() where TToken : class, IToken => (TToken)Current;

        public int Position { get; private set; } = -1;

        public TokenReader(IEnumerable<IToken> tokens) {
            this.tokens = tokens.ToArray();
            size = this.tokens.Length;
        }

        private TokenReader(IEnumerable<IToken> tokens, int position) : this(tokens) {
            Position = position == -1 ? -1 : position - 1;
            Read();
        }

        public bool Read() {
            bool hasNext = Position < size - 1;
            if (hasNext) {
                Current = tokens[++Position];
                return true;
            }
            return false;
        }

        public bool MoveTo(int position) {
            if (position > -1 && position < size) {
                Position = position;
                return true;
            }
            return false;
        }

        public TokenReader Clone() {
            return new TokenReader(tokens, Position);
        }
    }
}

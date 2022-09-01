using Assembler.Tokens;
using System;
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
#if DEBUG
            if (HasDuplicates(tokens, x => x, out var duplicates)) {
                throw new Exception("Token reader cannot have duplicates");
            }
            bool HasDuplicates<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, out IEnumerable<TKey> duplicates) {
                duplicates = source
                    .GroupBy(keySelector)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToArray();

                return duplicates.Any();
            }
#endif
            this.tokens = tokens.ToArray();
            size = this.tokens.Length;
        }

        public TokenReader(IEnumerable<IToken> tokens, int position) : this(tokens) {
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

        public IToken Peek() {
            bool hasNext = Position < size - 1;
            if (hasNext) {
                return tokens[Position + 1];
            }

            return null;
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

        public void ReplaceToken(IToken oldToken, IToken newToken) {
            var index = Array.IndexOf(tokens, oldToken);

            if (index == -1) {
                throw new Exception($"Token not found {oldToken}");
            }

            tokens[index] = newToken;
        }

        public List<IToken> GetTokens() => tokens.ToList();
    }
}

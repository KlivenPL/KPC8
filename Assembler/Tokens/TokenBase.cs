using Assembler.Readers;

namespace Assembler.Tokens {
    public interface IToken {
        TokenClass Class { get; }
        bool TryAccept(CodeReader reader);
        int CodePosition { get; }
        int LineNumber { get; }

        IToken DeepCopy();
    }

    abstract class TokenBase<TValue> : IToken {
        public abstract TValue Value { get; protected set; }
        public abstract TokenClass Class { get; }

        public int CodePosition { get; private set; }
        public int LineNumber { get; private set; }

        public abstract bool TryAccept(CodeReader reader);
        public abstract IToken DeepCopy();

        public TokenBase<TValue> AddDebugData(int position, int line) {
            CodePosition = position;
            LineNumber = line;
            return this;
        }

        public override string ToString() {
            return Value.ToString();
        }
    }
}

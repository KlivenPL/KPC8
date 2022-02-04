using Assembler.Readers;

namespace Assembler.Tokens {
    public interface IToken {
        TokenClass Class { get; }
        bool TryAccept(CodeReader reader);
    }

    abstract class TokenBase<TValue> : IToken {
        public abstract TValue Value { get; protected set; }
        public abstract TokenClass Class { get; }
        public abstract bool TryAccept(CodeReader reader);
    }
}

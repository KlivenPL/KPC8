using Assembler.Tokens;
using System;

namespace Assembler._Infrastructure {
    class InvalidTokenException : Exception {
        public InvalidTokenException(IToken recievedToken, TokenClass expectedTokenClass) {
            RecievedToken = recievedToken;
            ExpectedTokenClass = expectedTokenClass;
        }

        public IToken RecievedToken { get; init; }
        public TokenClass ExpectedTokenClass { get; init; }

        public ParserException ToParserException() {
            return ParserException.Create($"Expected {ExpectedTokenClass}, got {RecievedToken.Class}", RecievedToken);
        }
    }
}

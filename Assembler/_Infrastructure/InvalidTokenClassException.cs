using Assembler.Tokens;
using System;

namespace Assembler._Infrastructure {
    class InvalidTokenClassException : Exception {
        public InvalidTokenClassException(IToken recievedToken, TokenClass expectedTokenClass) {
            RecievedToken = recievedToken;
            ExpectedTokenClass = expectedTokenClass;
        }

        public IToken RecievedToken { get; init; }
        public TokenClass ExpectedTokenClass { get; init; }

        public ParserException ToParserException() {
            if (RecievedToken is IdentifierToken identifierToken) {
                return ParserException.Create($"Expected {ExpectedTokenClass}, got {RecievedToken.Class}{Environment.NewLine}(or identifier {identifierToken} not found)", RecievedToken);
            } else {
                return ParserException.Create($"Expected {ExpectedTokenClass}, got {RecievedToken.Class}", RecievedToken);
            }
        }
    }
}

using Assembler.Contexts.Regions;
using Assembler.Tokens;
using System;

namespace Assembler._Infrastructure {
    class InvalidTokenClassException : Exception {
        public InvalidTokenClassException(IToken recievedToken, TokenClass expectedTokenClass, ModuleRegion module, IRegion region) {
            RecievedToken = recievedToken;
            ExpectedTokenClass = expectedTokenClass;
            Module = module;
            Region = region;
        }

        public IToken RecievedToken { get; }
        public TokenClass ExpectedTokenClass { get; }
        public ModuleRegion Module { get; }
        public IRegion Region { get; }

        public ParserException ToParserException() {
            if (RecievedToken is IdentifierToken identifierToken) {
                return ParserException.Create($"Expected {ExpectedTokenClass}, got {RecievedToken.Class}{Environment.NewLine}(or identifier {identifierToken} not found)", RecievedToken);
            } else {
                return ParserException.Create($"Expected {ExpectedTokenClass}, got {RecievedToken.Class}", RecievedToken);
            }
        }
    }
}

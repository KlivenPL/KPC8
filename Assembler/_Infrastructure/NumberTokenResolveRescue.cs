using Assembler.Tokens;
using System;

namespace Assembler._Infrastructure {
    static class NumberTokenResolveRescue {
        private static Func<string, NumberToken> getNumberToken;

        public static bool IsInResolvableContext => getNumberToken != null;

        public static void BeginResolvableContext(Func<string, NumberToken> getNumberTokenFunc) {
            getNumberToken = getNumberTokenFunc;
        }

        public static void EndResolvableContext() {
            getNumberToken = null;
        }

        public static void ResolveOrThrow(NumberToken numberToken) {
            if (IsInResolvableContext) {
                try {
                    if (!numberToken.TryResolve(getNumberToken)) {
                        throw ParserException.Create("Number token not resolved", numberToken);
                    }
                } catch (Exception ex) when (ex is not ParserException) {
                    throw ParserException.Create($"Number token not resolved", numberToken);
                }
            } else {
                throw ParserException.Create("Number token not resolved", numberToken);
            }
        }
    }
}

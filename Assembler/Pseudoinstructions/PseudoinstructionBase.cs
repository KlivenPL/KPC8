using Assembler._Infrastructure;
using Assembler.Encoders;
using Assembler.Readers;
using Assembler.Tokens;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assembler.Pseudoinstructions {
    abstract class PseudoinstructionBase {
        public abstract PseudoinstructionType Type { get; }
        protected abstract IEnumerable<IEnumerable<BitArray>> ParseInner(TokenReader reader);
        protected InstructionEncoder InstructionEncoder { get; } = new InstructionEncoder();

        public BitArray[] Parse(TokenReader reader) {
            return ParseInner(reader).SelectMany(bitArray => bitArray).ToArray();
        }

        /*protected void ParseParameters(TokenReader reader, out IToken[] parsedTokens, params TokenClass[] tokenClasses) {
            var tmpParsedTokens = new List<IToken>();

            for (int i = 0; i < tokenClasses.Length; i++) {
                if (reader.Read() && reader.Current.Class == tokenClasses[i]) {
                    tmpParsedTokens.Add(reader.Current);
                } else {
                    throw ParserException.Create($"Invalid {Type} pseudoinstruction parameter. Expected class {tokenClasses[i]}, got {reader.Current.Class}", reader.Current);
                }
            }

            parsedTokens = tmpParsedTokens.ToArray();
        }*/

        protected void ParseParameters<T>(TokenReader reader, out T parsedToken) where T : IToken {
            parsedToken = ParseNextParameter<T>(reader);
        }

        protected void ParseParameters<T1, T2>(TokenReader reader, out T1 parsedToken1, out T2 parsedToken2) where T1 : IToken where T2 : IToken {
            parsedToken1 = ParseNextParameter<T1>(reader);
            parsedToken2 = ParseNextParameter<T2>(reader);
        }

        protected void ParseParameters<T1, T2, T3>(TokenReader reader, out T1 parsedToken1, out T2 parsedToken2, out T3 parsedToken3) where T1 : IToken where T2 : IToken where T3 : IToken {
            parsedToken1 = ParseNextParameter<T1>(reader);
            parsedToken2 = ParseNextParameter<T2>(reader);
            parsedToken3 = ParseNextParameter<T3>(reader);
        }

        protected void SplitWord(ushort word, out byte lower, out byte higher) {
            lower = (byte)(word & 0x00FF);
            higher = (byte)((word >> 8) & 0x00FF);
        }

        private T ParseNextParameter<T>(TokenReader reader) where T : IToken {
            if (reader.Read() && reader.Current is T parsed) {
                return parsed;
            } else {
                throw ParserException.Create($"Invalid {Type} pseudoinstruction parameter. Expected class {typeof(T).Name}, got {reader.Current.Class}", reader.Current);
            }
        }
    }
}

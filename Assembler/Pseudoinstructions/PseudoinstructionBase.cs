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

        protected void ParseParameters(TokenReader reader, out IToken[] parsedTokens, params TokenClass[] tokenClasses) {
            var tmpParsedTokens = new List<IToken>();

            for (int i = 0; i < tokenClasses.Length; i++) {
                if (reader.Read() && reader.Current.Class == tokenClasses[i]) {
                    tmpParsedTokens.Add(reader.Current);
                } else {
                    throw ParserException.Create($"Invalid {Type} pseudoinstruction parameter. Expected class {tokenClasses[i]}, got {reader.Current.Class}", reader.Current);
                }
            }

            parsedTokens = tmpParsedTokens.ToArray();
        }
    }
}

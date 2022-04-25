using Assembler._Infrastructure;
using Assembler.Contexts;
using Assembler.Readers;
using Assembler.Tokens;
using System.Collections;

namespace Assembler.Parsers {
    class PseudoinstructionParser {
        private readonly PseudoinstructionsContext pseudoinstructionsContext;

        public PseudoinstructionParser(PseudoinstructionsContext pseudoinstructionsContext) {
            this.pseudoinstructionsContext = pseudoinstructionsContext;
        }

        public BitArray[] Parse(TokenReader reader) {
            var identifier = reader.CastCurrent<IdentifierToken>();

            if (identifier.IsPseudoinstruction(out var pseudoinstructionType)) {
                return pseudoinstructionsContext.GetPseudoinstruction(pseudoinstructionType).Parse(reader);
            } else {
                throw ParserException.Create($"Identifier is not a pseudoinstruction", reader.Current);
            }
        }
    }
}

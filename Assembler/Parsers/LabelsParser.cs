using Assembler.Contexts;
using Assembler.Tokens;

namespace Assembler.Parsers {
    class LabelsParser {
        private readonly LabelsContext labels;

        public LabelsParser(LabelsContext labels) {
            this.labels = labels;
        }

        public bool TryParse(IdentifierToken token, out ushort? address) {
            if (labels.TryFindLabel(token.Value, out address)) {
                return true;
            }
            return false;
        }
    }
}

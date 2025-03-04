using System.Collections.Generic;

namespace Assembler.Contexts.Signatures {
    internal class KpcSignature {
        public string Name { get; init; }
        public KpcSignatureType Type { get; init; }
        public IEnumerable<KpcArgument> Arguments { get; init; }
    }
}

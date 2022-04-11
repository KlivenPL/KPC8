using Assembler.Tokens;
using System;
using System.Collections;

namespace Assembler._Infrastructure {
    class LabelNotResolvedException : Exception {
        // gdy kompiler znajdzie pseudoinstrukcję, tj albo po operacji, albo po fakcie, że immediate > byte lub immediate == label (sprawdzać, czy pseudoinstrukcja istnieje.)
        // labelki resolvować na samym końcu
        public ushort SizeInBytes { get; }
        public IdentifierToken PseudoinstructionToken { get; private set; }
        public IdentifierToken ArgumentToken { get; }
        public Func<ushort, BitArray[]> Resolve { get; }
        public ushort Address { get; private set; }

        public LabelNotResolvedException(IdentifierToken argumentToken, ushort sizeInBytes, Func<ushort, BitArray[]> resolve) {
            ArgumentToken = argumentToken;
            SizeInBytes = sizeInBytes;
            Resolve = resolve;
        }

        public void SetAddress(ushort address) {
            this.Address = address;
        }

        public void SetPseudoinstructionToken(IdentifierToken pseudoinstructionToken) {
            PseudoinstructionToken = pseudoinstructionToken;
        }
    }
}

using Assembler.Tokens;
using System;

namespace Assembler._Infrastructure {
    class ChangeToAssRegisterException : Exception {

        public ChangeToAssRegisterException(RegisterToken tokenToChange) {
            this.TokenToChange = tokenToChange;
        }

        public RegisterToken TokenToChange { get; }
    }
}

using Assembler.Tokens;
using System;
using System.Collections;

namespace Assembler._Infrastructure {
    class RegisterChangedToAssException : Exception {

        public RegisterChangedToAssException(ChangeToAssRegisterException originalException, BitArray instructionHigh, BitArray instructionLow) {
            ChangedToken = originalException.TokenToChange;
            InstructionHigh = instructionHigh;
            InstructionLow = instructionLow;
        }

        public RegisterToken ChangedToken { get; private set; }
        public BitArray InstructionHigh { get; private set; }
        public BitArray InstructionLow { get; private set; }
    }
}

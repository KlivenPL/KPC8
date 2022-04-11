using System;

namespace Assembler.DebugData {
    public class VariableSymbol : IDebugSymbol {

        public VariableSymbol() {
            throw new NotImplementedException();
        }

        public int SourceLine { get; init; }
        public string Name { get; init; }
        public ushort Value { get; init; }
    }
}

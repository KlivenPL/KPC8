using Assembler.DebugData;
using System.Collections.Generic;

namespace Runner.Configuration {
    public class DebugSessionConfiguration {
        public bool StopAtEntry { get; init; }
        public IEnumerable<IDebugSymbol> DebugSymbols { get; init; }
    }
}

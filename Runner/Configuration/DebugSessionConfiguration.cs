using Assembler.DebugData;
using System.Collections.Generic;

namespace Runner.Configuration {
    public class DebugSessionConfiguration {
        public IEnumerable<IDebugSymbol> DebugSymbols { get; init; }
    }
}

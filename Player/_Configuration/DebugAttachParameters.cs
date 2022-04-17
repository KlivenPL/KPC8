using Assembler.DebugData;
using Player._Configuration.Dtos;
using System.Collections;

namespace Player._Configuration {
    internal class DebugAttachParameters {
        public int ServerPort { get; init; }
        public string SourceFilePath { get; init; }
        public BitArray[] CompiledProgram { get; init; }
        public IEnumerable<IDebugSymbol> DebugSymbols { get; init; }
        public KPC8ConfigurationDto KPC8ConfigurationDto { get; init; }
    }
}

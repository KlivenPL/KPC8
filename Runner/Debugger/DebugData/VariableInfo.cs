using System.Collections;

namespace Runner.Debugger.DebugData {
    public class VariableInfo {
        public string Name { get; init; }
        public string Value { get; init; }
        public BitArray ValueRaw { get; init; }
        public string MemoryReference { get; init; }
    }
}

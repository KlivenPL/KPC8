using System.Collections.Generic;

namespace Runner.Debugger.DebugData {
    public class ScopeInfo {
        public string Name { get; init; }
        public IEnumerable<VariableInfo> Variables { get; init; }
        public int VariablesReference { get; init; }
    }
}

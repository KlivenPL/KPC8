using System.Collections.Generic;

namespace Runner.Debugger.DebugData {
    public class StackFrameInfo {
        public int Line { get; init; }
        public IEnumerable<ScopeInfo> Scopes { get; init; }
    }
}

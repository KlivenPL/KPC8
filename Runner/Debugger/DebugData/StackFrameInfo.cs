using System.Collections.Generic;

namespace Runner.Debugger.DebugData {
    public class StackFrameInfo {
        public IEnumerable<ScopeInfo> Scopes { get; init; }
    }
}

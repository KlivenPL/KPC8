using System.Collections.Generic;

namespace Runner.Debugger.DebugData {
    public class DebugInfo {
        public int HitBreakpointId { get; init; }
        public IEnumerable<StackFrameInfo> Frames { get; init; }
    }
}

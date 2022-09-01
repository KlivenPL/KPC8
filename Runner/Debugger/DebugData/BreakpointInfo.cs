using Runner.Debugger.DebugData.Internal;

namespace Runner.Debugger.DebugData {
    public class BreakpointInfo {

        public int Id { get; init; }
        public string FilePath { get; init; }
        public int Line { get; init; }
        public int Column { get; init; }
        public int EndColumn { get; init; }

        internal BreakpointInfo(Breakpoint breakpoint) {
            Id = breakpoint.Id;
            FilePath = breakpoint.Symbol.FilePath;
            Line = breakpoint.Symbol.Line;
            Column = breakpoint.Symbol.ColumnStart;
            EndColumn = breakpoint.Symbol.ColumnEnd;
        }
    }
}

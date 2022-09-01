using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Messages;
using Runner.Debugger.DebugData;

namespace DebugAdapter.Mappers {
    internal static class BreakpointMapper {

        public static Breakpoint ToVerifiedBreakpoint(this BreakpointInfo bpi) {
            return new Breakpoint {
                Id = bpi.Id,
                Line = bpi.Line,
                Column = bpi.Column,
                EndColumn = bpi.EndColumn,
                Verified = true,
                Source = new Source { Path = bpi.FilePath },
            };
        }

        public static Breakpoint ToUnverifiedBreakpoint(this SourceBreakpoint sbp, string filePath) {
            return new Breakpoint {
                Line = sbp.Line,
                Verified = false,
                Source = new Source { Path = filePath },
            };
        }
    }
}

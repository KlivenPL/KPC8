using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Messages;
using Runner.Debugger.DebugData;

namespace DebugAdapter.Mappers {
    internal static class BreakpointMapper {

        public static Breakpoint ToVerifiedBreakpoint(this BreakpointInfo bpi, Source source) {
            return new Breakpoint {
                Id = bpi.Id,
                Line = bpi.Line,
                Column = bpi.Column,
                EndColumn = bpi.EndColumn,
                Verified = true,
                Source = source,
            };
        }

        public static Breakpoint ToUnverifiedBreakpoint(this SourceBreakpoint sbp) {
            return new Breakpoint {
                Line = sbp.Line,
                Verified = false,
            };
        }
    }
}

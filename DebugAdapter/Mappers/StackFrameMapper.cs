using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Messages;
using Runner.Debugger.DebugData;

namespace DebugAdapter.Mappers {
    internal static class StackFrameMapper {
        private static int StackFrameId = 0;
        public static StackFrame ToStackFrame(this StackFrameInfo sfi, StackTraceArguments arguments) {
            if (arguments.ThreadId != 0) {
                return new StackFrame();
            }

            return new StackFrame {
                Id = StackFrameId,
                Line = sfi.Line,
                Source = new Source { Path = sfi.FilePath }
            };
        }
    }
}

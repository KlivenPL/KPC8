using Assembler.DebugData;

namespace Runner.Debugger.DebugData.Internal {
    internal class Breakpoint {
        internal int Id { get; init; }
        internal ExecutableSymbol Symbol { get; init; }
    }
}

using Assembler.DebugData;
using Runner.Debugger.DebugData;
using Runner.Debugger.DebugData.Internal;
using System.Collections.Generic;
using System.Linq;

namespace Runner.Debugger.Managers {
    internal class BreakpointManager {
        private readonly Breakpoint[] possibleBps;
        private readonly Dictionary<ushort, int> loAddressToPossibleBpId;

        private Breakpoint[] placedBps = System.Array.Empty<Breakpoint>();
        private Dictionary<ushort, int> loAddressToPlacedBpId = new Dictionary<ushort, int>();

        internal BreakpointManager(IEnumerable<IDebugSymbol> debugSymbols) {
            possibleBps = debugSymbols
                .OfType<ExecutableSymbol>()
                .Where(x => x.Line >= 0)
                .OrderBy(x => x.LoAddress)
                .Select((x, i) => new Breakpoint { Id = i, Symbol = x })
                .ToArray();

            loAddressToPossibleBpId = possibleBps.ToDictionary(x => x.Symbol.LoAddress, x => x.Id);
        }

        public IEnumerable<BreakpointInfo> GetPossibleBreakpointLocations() {
            return possibleBps.Select(x => new BreakpointInfo(x));
        }

        public int GetLineOfBreakpoint(int? breakpointId) {
            if (breakpointId == null) {
                return possibleBps.Last().Symbol.Line;
            }
            return possibleBps.First(x => x.Id == breakpointId).Symbol.Line;
        }

        public IEnumerable<BreakpointInfo> SetBreakpoints(IEnumerable<(int line, int column)> proposedBreakpoints) {
            placedBps = possibleBps.Where(s => proposedBreakpoints.Any(pb => pb.line == s.Symbol.Line /*&& pb.column == s.Symbol.ColumnStart*/)).ToArray();

            if (placedBps.Any()) {
                loAddressToPlacedBpId = placedBps.ToDictionary(x => x.Symbol.LoAddress, x => x.Id);
            } else {
                loAddressToPlacedBpId.Clear();
            }

            return placedBps.Select(x => new BreakpointInfo(x));
        }

        public bool IsBreakpointHit(ushort loAddress, out int? breakpointId) {
            if (loAddressToPlacedBpId.TryGetValue(loAddress, out var tmpBreakpointId) == true) {
                breakpointId = tmpBreakpointId;
                return true;
            }

            breakpointId = null;
            return false;
        }

        public bool CanPauseHere(ushort loAddress, out int? breakpointId) {
            if (loAddressToPossibleBpId.TryGetValue(loAddress, out var tmpBreakpointId)) {
                breakpointId = tmpBreakpointId;
                return true;
            }

            breakpointId = null;
            return false;
        }

        internal ushort? GetNextPossibleBreakpointAddressInAddressOrder(ushort currentAddress) {
            return possibleBps.FirstOrDefault(x => x.Symbol.LoAddress > currentAddress)?.Symbol.LoAddress;
        }
    }
}
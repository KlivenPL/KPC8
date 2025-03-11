using _Infrastructure.Paths;
using Assembler.DebugData;
using Runner.Debugger.DebugData;
using Runner.Debugger.DebugData.Internal;
using System.Collections.Generic;
using System.Linq;

namespace Runner.Debugger.Managers {
    internal class BreakpointManager {
        private readonly Breakpoint[] possibleBps;
        private readonly Dictionary<ushort, int> loAddressToPossibleBpId;

        private Dictionary<string, List<Breakpoint>> placedFileBps = new();
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

        public void GetBreakpointData(int? breakpointId, out string filePath, out int line) {
            var bp = breakpointId == null ? possibleBps.Last() : possibleBps.First(x => x.Id == breakpointId);
            filePath = bp.Symbol.FilePath;
            line = bp.Symbol.Line;
        }

        public IEnumerable<BreakpointInfo> SetBreakpoints(string filePath, IEnumerable<(int line, int column)> proposedBreakpoints) {
            List<Breakpoint> fileBreakpoints = new();

            if (placedFileBps.ContainsKey(filePath)) {
                placedFileBps.Remove(filePath);
            }

            foreach (var proposedBreakpoint in proposedBreakpoints) {
                var possibleBreakpoint = possibleBps.FirstOrDefault(x =>
                    proposedBreakpoint.line == x.Symbol.Line &&
                    filePath.ComparePath(x.Symbol.FilePath));

                if (possibleBreakpoint == null) {
                    continue;
                }

                fileBreakpoints.Add(possibleBreakpoint);
            }

            //foreach (var possibleBreakpoint in fileBreakpoints) {
            //    if (placedFileBps.ContainsKey(possibleBreakpoint.Symbol.FilePath)) {
            //        placedFileBps[possibleBreakpoint.Symbol.FilePath].Add(possibleBreakpoint);
            //    } else {
            //        placedFileBps.Add(possibleBreakpoint.Symbol.FilePath, new List<Breakpoint> { possibleBreakpoint });
            //    }
            //}

            if (fileBreakpoints.Any()) {
                placedFileBps.Add(filePath, fileBreakpoints);
            }

            if (placedFileBps.Any()) {
                loAddressToPlacedBpId = placedFileBps.Values.SelectMany(x => x).ToDictionary(x => x.Symbol.LoAddress, x => x.Id);
            } else {
                loAddressToPlacedBpId.Clear();
            }

            return placedFileBps.Values.SelectMany(x => x).Select(x => new BreakpointInfo(x));

            //var fileBps = possibleBps
            //    .Where(s => 
            //        proposedBreakpoints.Any(pb => pb.line == s.Symbol.Line && pb.filePath.ComparePath(s.Symbol.FilePath)))
            //    .ToArray();

            //if(placedFileBps.ContainsKey())

            //if (placedBps.Any()) {
            //    loAddressToPlacedBpId = placedBps.ToDictionary(x => x.Symbol.LoAddress, x => x.Id);
            //} else {
            //    loAddressToPlacedBpId.Clear();
            //}

            //return placedBps.Select(x => new BreakpointInfo(x));
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
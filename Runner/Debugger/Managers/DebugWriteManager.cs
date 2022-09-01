using Assembler.DebugData;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Runner.Debugger.Managers {
    internal class DebugWriteManager {
        private readonly DebugWriteSymbol[] _debugWrites;

        public delegate bool TryGetRegisterDelegate(string name, out string value);
        public delegate bool TryGetConstantDelegate(string name, string filePath, int line, out string value);
        public delegate bool TryEvaluateExpressionDelegate(string expression, string filePath, int line, out string value);

        internal DebugWriteManager(IEnumerable<IDebugSymbol> debugSymbols) {
            _debugWrites = debugSymbols
                .OfType<DebugWriteSymbol>()
                .ToArray();
        }

        public bool IsDebugWriteHit(ushort loAddress, out IEnumerable<DebugWriteSymbol> debugWrites) {
            debugWrites = _debugWrites.Where(x => x.LoAddress == loAddress);
            return debugWrites.Any();
        }

        public IEnumerable<(string, string, int)> GetValues(IEnumerable<DebugWriteSymbol> debugWriteSymbols, TryGetRegisterDelegate tryGetRegisterValue, TryGetConstantDelegate tryGetConstantValue, TryEvaluateExpressionDelegate tryEvaluateExpression) {
            return debugWriteSymbols.Select(debugWriteSymbol =>
                (Regex.Replace(debugWriteSymbol.Value, "{(.*?)}", match => {
                    var line = debugWriteSymbol.Line;
                    var matchValue = match.Groups[1].Value;

                    if (matchValue.Contains('.')) {
                        return "(unsupported expression)";
                    }

                    if (matchValue.StartsWith('$') && tryGetRegisterValue(matchValue[1..], out var registerValue)) {
                        return registerValue;
                    }

                    if (matchValue.StartsWith('%') && matchValue.EndsWith('%') && matchValue.Length > 2) {
                        if (tryEvaluateExpression(matchValue[1..^1], debugWriteSymbol.FilePath, debugWriteSymbol.Line, out var evalResult)) {
                            return evalResult;
                        }
                        return $"({evalResult})";
                    }

                    if (tryGetConstantValue(matchValue, debugWriteSymbol.FilePath, debugWriteSymbol.Line, out var constantValue)) {
                        return constantValue;
                    }
                    return "(undefined)";

                }, RegexOptions.IgnoreCase), debugWriteSymbol.FilePath, debugWriteSymbol.Line));
        }
    }
}
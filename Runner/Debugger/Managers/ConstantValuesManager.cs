using Assembler.DebugData;
using Runner.Debugger.DebugData;
using System.Collections.Generic;
using System.Linq;

namespace Runner.Debugger.Managers {
    internal class ConstantValuesManager {
        private readonly List<ConstantValueSymbol> _constantValues;

        internal ConstantValuesManager(IEnumerable<IDebugSymbol> debugSymbols) {
            _constantValues = debugSymbols
                .OfType<ConstantValueSymbol>()
                .Where(x => x.Line >= 0)
                .ToList();
        }

        public IEnumerable<ConstantValueInfo> GetValues(IEnumerable<VariableInfo> variables) {
            return _constantValues.Select(cv => cv.IsRegisterAlias ? new ConstantValueInfo(cv.Line, cv.Name, cv.Value, variables) : new ConstantValueInfo(cv.Line, cv.Name, cv.Value));
        }
    }
}
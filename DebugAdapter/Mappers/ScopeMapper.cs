using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Messages;
using Runner.Debugger.DebugData;

namespace DebugAdapter.Mappers {
    internal static class ScopeMapper {
        public static Scope ToScope(this ScopeInfo si/*, Source source*/) {
            return new Scope {
                Name = si.Name,
                VariablesReference = si.VariablesReference,
                // Source = source,
                PresentationHint = Scope.PresentationHintValue.Locals,
            };
        }
    }
}

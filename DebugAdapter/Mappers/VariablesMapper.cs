using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Messages;
using Runner.Debugger.DebugData;

namespace DebugAdapter.Mappers {
    internal static class VariablesMapper {
        public static Variable ToProtocolVariable(this VariableInfo vi) {
            return new Variable {
                Name = vi.Name,
                Value = vi.Value,
                MemoryReference = vi.MemoryReference,
            };
        }
    }
}

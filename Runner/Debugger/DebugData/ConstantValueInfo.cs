using Infrastructure.BitArrays;
using System.Collections.Generic;
using System.Linq;

namespace Runner.Debugger.DebugData {
    public class ConstantValueInfo {

        public int Line { get; init; }
        public string Name { get; init; }
        public string Value { get; init; }
        public ushort? ValueRaw { get; init; }

        public bool IsRegisterAlias { get; init; }
        public string RegisterName { get; init; }

        internal ConstantValueInfo(int line, string name, string value) {
            Line = line;
            Name = name;
            Value = value;
            if (ushort.TryParse(value, out ushort valueRaw)) {
                ValueRaw = valueRaw;
            }
        }

        internal ConstantValueInfo(int line, string name, string registerName, IEnumerable<VariableInfo> variables) {
            Line = line;
            Name = name;
            var reg = variables.FirstOrDefault(x => x.Name == registerName);
            Value = reg?.Value;
            ValueRaw = reg?.ValueRaw?.ToUShortLE();
            IsRegisterAlias = reg != null;
            RegisterName = registerName;
        }
    }
}

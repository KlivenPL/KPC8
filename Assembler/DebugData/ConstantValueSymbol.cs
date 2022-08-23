using Newtonsoft.Json;

namespace Assembler.DebugData {
    public class ConstantValueSymbol : IDebugSymbol {

        public ConstantValueSymbol(int line, string name, string value, bool isRegisterAlias) {
            Line = line;
            Name = name;
            Value = value;
            IsRegisterAlias = isRegisterAlias;
        }

        [JsonProperty("l")]
        public int Line { get; init; }

        [JsonProperty("n")]
        public string Name { get; init; }

        [JsonProperty("v")]
        public string Value { get; init; }

        [JsonProperty("isReg")]
        public bool IsRegisterAlias { get; init; }
    }
}

using Newtonsoft.Json;

namespace Assembler.DebugData {
    public class DebugWriteSymbol : IDebugSymbol {

        public DebugWriteSymbol(ushort loAddress, int line, string value) {
            LoAddress = loAddress;
            Line = line;
            Value = value;
        }

        [JsonProperty("l")]
        public int Line { get; init; }

        [JsonProperty("addr")]
        public ushort LoAddress { get; init; }

        [JsonProperty("v")]
        public string Value { get; init; }
    }
}

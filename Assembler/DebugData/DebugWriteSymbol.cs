using Newtonsoft.Json;

namespace Assembler.DebugData {
    public class DebugWriteSymbol : IDebugSymbol {

        public DebugWriteSymbol(string filePath, ushort loAddress, int line, string value) {
            FilePath = filePath;
            LoAddress = loAddress;
            Line = line;
            Value = value;
        }

        [JsonProperty("f")]
        public string FilePath { get; init; }

        [JsonProperty("l")]
        public int Line { get; init; }

        [JsonProperty("addr")]
        public ushort LoAddress { get; init; }

        [JsonProperty("v")]
        public string Value { get; init; }
    }
}

using Assembler.Tokens;
using Newtonsoft.Json;
using System;

namespace Assembler.DebugData {
    public class ExecutableSymbol : IDebugSymbol {
        private ushort? loAddress = null;

        internal ExecutableSymbol(IdentifierToken identifierToken) {
            Line = identifierToken.LineNumber;
            ColumnStart = identifierToken.CodePosition;
            ColumnEnd = identifierToken.CodePosition + identifierToken.Value.Trim().Length;
        }

        internal ExecutableSymbol(IdentifierToken identifierToken, ushort loAddress) : this(identifierToken) {
            Resolve(loAddress);
        }

        [JsonProperty("l")]
        public int Line { get; init; }

        [JsonProperty("cs")]
        public int ColumnStart { get; init; }

        [JsonProperty("ce")]
        public int ColumnEnd { get; init; }

        [JsonProperty("addr")]
        public ushort LoAddress => loAddress ?? throw new InvalidOperationException();

        internal void Resolve(ushort loAddress) {
            this.loAddress = loAddress;
        }
    }
}

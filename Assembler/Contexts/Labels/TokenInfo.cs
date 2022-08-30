using Assembler.Tokens;

namespace Assembler.Contexts.Labels {
    public class TokenInfo {
        public TokenInfo(string name, IToken value) {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public IToken Value { get; set; }
    }
}

using Assembler._Infrastructure;
using Assembler.Contexts.Labels;
using Assembler.Readers;
using Assembler.Tokens;
using System.Collections.Generic;
using System.Linq;

namespace Assembler.Contexts.Regions {
    internal class UserDefinedRegion : IRegion {
        private readonly List<LabelInfo> labels;
        private readonly List<TokenInfo> tokens;

        public UserDefinedRegion(string name) {
            Name = name;
            labels = new List<LabelInfo>();
            tokens = new List<TokenInfo>();
        }

        public string Name { get; }
        public bool IsReserved => false;
        public bool IsExported { get; private set; }

        public static IRegion PreParse(TokenReader reader) {
            if (reader.Current is not RegionToken regionToken) {
                throw new OtherInnerException("Given token is not region token");
            }

            var region = new UserDefinedRegion(regionToken.Value);

            do {
                if (reader.Current is LabelToken labelToken) {
                    region.InsertLabel(labelToken);

                    if (!reader.Read() || reader.Current is not IdentifierToken identifierToken || !identifierToken.IsInstruction(out _) && !identifierToken.IsPseudoinstruction(out _)) {
                        throw new OtherInnerException($"Instruction or pseudoinstruction expected after the label {labelToken.Value}, got: {reader.Current.Class}");
                    }

                }
            } while (reader.Read() && reader.Current is not RegionToken);

            return region;
        }

        public void SetAsExported() {
            if (IsExported) {
                throw new OtherInnerException($"Region {Name} is already exported");
            }

            IsExported = true;
        }

        public void InsertToken(string name, IToken token) {
            if (tokens.Any(x => x.Name == name)) {
                throw new OtherInnerException($"Duplicated identifier: {name} in region {Name}");
            }

            var newTokenInfo = new TokenInfo(name, token);
            tokens.Add(newTokenInfo);
        }

        public TokenInfo GetToken(string tokenName) {
            return tokens.FirstOrDefault(x => x.Name == tokenName) ?? throw new OtherInnerException($"Token {tokenName} not found in region {Name}");
        }

        public LabelInfo GetLabel(string labelName) {
            return labels.FirstOrDefault(x => x.Name == labelName) ?? throw new OtherInnerException($"Label {labelName} not found in region {Name}");
        }

        public LabelInfo GetFirstLabel() {
            return labels.FirstOrDefault() ?? throw new OtherInnerException($"Region {Name} does not have any labels. Define :main label as a program entry point");
        }

        private void InsertLabel(LabelToken labelToken) {
            if (labels.Any(x => x.Name == labelToken.Value)) {
                throw new OtherInnerException($"Duplicated label: {labelToken.Value} in region {Name}");
            }

            labels.Add(new LabelInfo(labelToken.Value, null));
        }
    }
}

using Assembler._Infrastructure;
using Assembler.Contexts.Labels;
using Assembler.Parsers;
using Assembler.Readers;
using Assembler.Tokens;
using System.Collections.Generic;
using System.Linq;

namespace Assembler.Contexts.Regions {
    internal class ConstRegion : IRegion {
        private readonly List<TokenInfo> tokens;

        public ConstRegion() {
            tokens = new List<TokenInfo>();
        }

        public string Name => RegionParser.ConstRegionName;
        public bool IsReserved => true;

        public static IRegion PreParse(TokenReader reader) {
            if (reader.Current is not RegionToken) {
                throw new OtherInnerException("Given token is not region token");
            }

            var region = new ConstRegion();

            do {
                if (reader.Current is LabelToken) {
                    throw new OtherInnerException($"Cannot declare labels in reserved {region.Name} region");
                }
            } while (reader.Read() && reader.Current is not RegionToken);

            return region;
        }

        public TokenInfo GetToken(string tokenName) {
            return tokens.FirstOrDefault(x => x.Name == tokenName) ?? throw new OtherInnerException($"Region {Name} does not define {tokenName} token");
        }

        public void InsertToken(string name, IToken token) {
            if (tokens.Any(x => x.Name == name)) {
                throw new OtherInnerException($"Duplicated identifier: {name} in region {Name}");
            }

            var newTokenInfo = new TokenInfo(name, token);
            tokens.Add(newTokenInfo);
        }

        public LabelInfo GetLabel(string labelName) {
            throw new OtherInnerException($"{RegionParser.ConstRegionName} region cannot contain labels");
        }
    }
}

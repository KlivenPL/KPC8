using Assembler._Infrastructure;
using Assembler.Contexts.Labels;
using Assembler.Parsers;
using Assembler.Readers;
using Assembler.Tokens;
using System.Collections.Generic;
using System.Linq;

namespace Assembler.Contexts.Regions {
    internal class ModuleRegion : IRegion {
        private readonly ConstRegion constRegion;
        private readonly List<UserDefinedRegion> userDefinedRegions;

        private IEnumerable<IRegion> AllRegions => userDefinedRegions.Concat(new IRegion[] { constRegion });

        public ModuleRegion(string name, ConstRegion constRegion) {
            this.constRegion = constRegion;
            Name = name;
            userDefinedRegions = new List<UserDefinedRegion>();
        }

        public string Name { get; }
        public bool IsReserved => true;

        public static IRegion PreParse(TokenReader reader, ConstRegion constRegion) {
            if (reader.Current is not RegionToken) {
                throw new OtherInnerException("Given token is not region token");
            }

            if (!reader.Read() || reader.Current is not IdentifierToken identifierToken) {
                throw new OtherInnerException($"Expected module identifier (name) after module definition, got {reader.Current.Class}");
            }

            var name = identifierToken.Value;
            var region = new ModuleRegion(name, constRegion);

            do {
                if (reader.Current is LabelToken) {
                    throw new OtherInnerException($"Cannot declare labels in reserved module {name} region");
                }
            } while (reader.Read() && reader.Current is not RegionToken);

            return region;
        }

        public IRegion GetRegion(string regionName) {
            return AllRegions.FirstOrDefault(x => x.Name == regionName) ?? throw new OtherInnerException($"Region {regionName} does not exist in module {Name}");
        }

        public IRegion GetExportedRegion(string regionName) {
            var region = userDefinedRegions.FirstOrDefault(x => x.Name == regionName) ?? throw new OtherInnerException($"Region {regionName} does not exist in module {Name}");
            if (!region.IsExported) {
                throw new OtherInnerException($"Region {regionName} is not exported from module {Name}");
            }

            return region;
        }

        /* public IRegion TryGetExportedRegion(string regionName) {
             return userDefinedRegions
                 .Where(x => x.IsExported)
                 .Concat(new IRegion[] { constRegion });
         }*/

        public void AddUserDefinedRegion(UserDefinedRegion region) {
            if (userDefinedRegions.Any(r => r.Name == region.Name)) {
                throw new OtherInnerException($"Duplicated region {region.Name} in module {Name}");
            }

            userDefinedRegions.Add(region);
        }

        public void InsertToken(string name, IToken token) {
            throw new OtherInnerException($"Constant definition is not allowd in reserved {RegionParser.ModuleRegionName} regions");
        }

        public LabelInfo GetLabel(string labelName) {
            throw new OtherInnerException($"{RegionParser.ModuleRegionName} region cannot contain labels");
        }

        public TokenInfo GetToken(string tokenName) {
            throw new OtherInnerException($"{RegionParser.ModuleRegionName} region cannot contain tokens");
        }
    }
}

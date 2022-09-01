using Assembler._Infrastructure;
using Assembler.Contexts.Regions;
using Assembler.Readers;
using Assembler.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assembler.Parsers {
    internal class RegionPreParser {
        public const string GlobalRegionName = "@global";
        public const string ConstRegionName = "@const";
        public const string ModuleRegionName = "@module";

        private static readonly string[] reservedRegions = {
            GlobalRegionName, ConstRegionName, ModuleRegionName,
        };

        public ConstRegion PreParseConstRegion(TokenReader reader) {
            if (reader.Current is not RegionToken regionToken) {
                throw new OtherInnerException($"Could not parse {ConstRegionName} region: given token is not region token");
            }

            var firstRegion = PreParseNextRegion(reader, regionToken.Value, null, null);
            if (firstRegion is not ConstRegion cr) {
                throw new OtherInnerException($"{ConstRegionName} must be the first defined region");
            }

            return cr;
        }

        public ModuleRegion PreParseModuleRegion(TokenReader reader, ConstRegion constRegion) {
            if (reader.Current is not RegionToken regionToken) {
                throw new OtherInnerException($"Could not parse {ModuleRegionName} region: given token is not region token");
            }

            var firstRegion = PreParseNextRegion(reader, regionToken.Value, constRegion, null);
            if (firstRegion is not ModuleRegion mr) {
                throw new OtherInnerException($"{ModuleRegionName} must be the second defined region in main file or first defined region in module file");
            }

            return mr;
        }

        public void PreParseAllRegions(TokenReader reader, out string mainLabelIdentifier, out ConstRegion constRegion, out List<ModuleRegion> modules) {
            if (reader.Current is not RegionToken) {
                throw new OtherInnerException("Could not parse required regions: given token is not region token");
            }

            constRegion = PreParseConstRegion(reader);
            var prevModuleRegion = PreParseModuleRegion(reader, constRegion);

            mainLabelIdentifier = null;

            modules = new List<ModuleRegion> {
                prevModuleRegion
            };

            while (reader.Current is not null && reader.Current is RegionToken regionToken) {
                var nextRegion = PreParseNextRegion(reader, regionToken.Value, constRegion, prevModuleRegion);

                if (nextRegion is ModuleRegion mr) {
                    CheckForUnexportedREgions(prevModuleRegion);
                    prevModuleRegion = mr;
                    modules.Add(mr);
                } else if (nextRegion is UserDefinedRegion udr) {
                    if (mainLabelIdentifier == null) {
                        var mainLabelInfo = udr.GetFirstLabel();
                        mainLabelIdentifier = $"{prevModuleRegion.Name}.{udr.Name}.{mainLabelInfo.Name}";
                    }
                    prevModuleRegion.AddUserDefinedRegion(udr);
                } else if (nextRegion is ConstRegion) {
                    throw new OtherInnerException($"Only single {ConstRegionName} region can be defined");
                }
            }

            CheckForUnexportedREgions(prevModuleRegion);

            if (HasDuplicates(modules.OfType<ModuleRegion>(), x => x.Name, out var moduleDuplicates)) {
                throw new OtherInnerException($"Duplicates of modules: {string.Join(',', moduleDuplicates)} were found");
            }
        }

        private static void CheckForUnexportedREgions(ModuleRegion prevModuleRegion) {
            if (prevModuleRegion.AnyAwaitingRegionForExport(out var unexportedRegions)) {
                throw new OtherInnerException($"Could not find regions: {string.Join(',', unexportedRegions)} to export in module {prevModuleRegion.Name}");
            }
        }

        private IRegion PreParseNextRegion(TokenReader reader, string regionName, ConstRegion constRegion, ModuleRegion prevModuleRegion) {
            return regionName switch {
                GlobalRegionName => throw new OtherInnerException($"Region {GlobalRegionName} is reserved for compiler only and cannot be used"),
                ConstRegionName => ConstRegion.PreParse(reader),
                ModuleRegionName => ModuleRegion.PreParse(reader, constRegion),
                _ => !regionName.StartsWith('@')
                    ? UserDefinedRegion.PreParse(reader, prevModuleRegion.AwaitingRegionsForExport.Contains(regionName))
                    : throw new OtherInnerException($"Only reserved regions can start with '@'. Reserved regions are: {string.Join(", ", reservedRegions)}")
            };
        }

        private bool HasDuplicates<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, out IEnumerable<TKey> duplicates) {
            duplicates = source
                .GroupBy(keySelector)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToArray();

            return duplicates.Any();
        }
    }
}

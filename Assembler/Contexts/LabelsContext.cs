using Assembler.Readers;
using Assembler.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assembler.Contexts {
    class LabelsContext {
        private class LabelInfo {
            public LabelInfo(string name, ushort? address) {
                Name = name;
                Address = address;
            }

            public string Name { get; set; }
            public ushort? Address { get; set; }
        }

        public const string GlobalRegion = "@global";
        private readonly Dictionary<string, List<LabelInfo>> regionedLabels;

        public string CurrentRegion { get; set; } = GlobalRegion;

        public LabelsContext() {
            regionedLabels = new Dictionary<string, List<LabelInfo>>();
            ClearRegionedLabels();
        }

        public bool TryParseAllRegionsAndLabels(TokenReader reader) {
            string currentRegion = GlobalRegion;
            string currentLabel = null;
            ClearRegionedLabels();

            while (reader.Read()) {
                if (reader.Current is RegionToken regionToken) {
                    if (currentRegion != GlobalRegion && currentLabel == null) {
                        return false;
                    }

                    if (regionedLabels.ContainsKey(regionToken.Value)) {
                        return false;
                    }

                    regionedLabels.Add(regionToken.Value, new List<LabelInfo>());
                    currentRegion = regionToken.Value;
                    currentLabel = null;
                } else if (reader.Current is LabelToken labelToken) {
                    var prevLabels = regionedLabels[currentRegion];
                    if (prevLabels.Select(l => l.Name).Contains(labelToken.Value)) {
                        return false;
                    }

                    prevLabels.Add(new LabelInfo(labelToken.Value, null));
                    currentLabel = labelToken.Value;
                }
            }

            return true;
        }

        public bool TryFindLabel(string identifier, out ushort? address) {
            address = null;

            if (!TryGetRegionAndLabel(identifier, out var region, out var label)) {
                return false;
            }

            if (regionedLabels.TryGetValue(region, out var labels)) {
                var tuple = labels.FirstOrDefault(l => l.Name == label);
                if (tuple != default) {
                    address = tuple.Address;
                    return true;
                }
            }

            return false;
        }

        public void ResolveLabel(string labelDef, ushort address) {
            if (!TryFindLabel(labelDef, out var oldAddress)) {
                throw new Exception("Label not found");
            } else if (oldAddress.HasValue) {
                throw new Exception("Label already resolved");
            }

            regionedLabels[CurrentRegion].First(x => x.Name == labelDef).Address = address;
        }

        private bool TryGetRegionAndLabel(string identifier, out string region, out string label) {
            var split = identifier.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            region = CurrentRegion;
            label = null;

            if (split.Length == 1) {
                label = split[0];
            } else if (split.Length == 2) {
                region = split[0];
                label = split[1];
            } else {
                return false;
            }

            return true;
        }

        private void ClearRegionedLabels() {
            regionedLabels.Clear();
            regionedLabels.Add(GlobalRegion, new List<LabelInfo>());
        }
    }
}

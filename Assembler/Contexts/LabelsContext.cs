using Assembler.Readers;
using Assembler.Tokens;
using System.Collections.Generic;
using System.Linq;

namespace Assembler.Contexts {
    class LabelsContext {
        public const string GlobalRegion = "@global";
        private readonly Dictionary<string, List<(string name, ushort? address)>> regionedLabels;

        /* public string CurrentRegion { get; private set; }
         public string CurrentLabel { get; private set; }*/

        public LabelsContext() {
            regionedLabels = new Dictionary<string, List<(string name, ushort? address)>>();
            regionedLabels.Add(GlobalRegion, new List<(string name, ushort? address)>());
            //TryStartRegion(GlobalRegion);
        }

        public bool TryParseAllRegionsAndLabels(TokenReader reader) {
            string currentRegion = GlobalRegion;
            string currentLabel = null;
            regionedLabels.Clear();

            while (reader.Read()) {
                if (reader.Current is RegionToken regionToken) {
                    if (currentRegion != GlobalRegion && currentLabel == null) {
                        return false;
                    }

                    if (regionedLabels.ContainsKey(regionToken.Value)) {
                        return false;
                    }

                    regionedLabels.Add(regionToken.Value, new List<(string name, ushort? address)>());
                    currentRegion = regionToken.Value;
                    currentLabel = null;
                } else if (reader.Current is LabelToken labelToken) {
                    var prevLabels = regionedLabels[currentRegion];
                    if (prevLabels.Select(l => l.name).Contains(labelToken.Value)) {
                        return false;
                    }

                    prevLabels.Add((labelToken.Value, null));
                    currentLabel = labelToken.Value;
                }
            }

            return true;
        }

        /*        public bool TryStartRegion(string newRegion) {
                    if (regionedLabels.ContainsKey(newRegion)) {
                        CurrentRegion = newRegion;
                        CurrentLabel = null;
                        return true;
                    }

                    return false;
                }

                public bool TryStartLabel(string label) {
                    var currentLabels = regionedLabels[CurrentRegion];
                    if (currentLabels.Contains(label)) {
                        CurrentLabel = label;
                        return true;
                    }
                    return false;
                }*/

        public bool TryFindLabel(string identifier, out ushort? address) {
            address = null;
            var split = identifier.Split(new string[] { "." }, System.StringSplitOptions.RemoveEmptyEntries);
            var region = GlobalRegion;
            string label;

            if (split.Length == 1) {
                label = split[0];
            } else if (split.Length == 2) {
                region = split[0];
                label = split[1];
            } else {
                return false;
            }

            if (regionedLabels.TryGetValue(region, out var labels)) {
                var tuple = labels.FirstOrDefault(l => l.name == label);
                if (tuple != default) {
                    address = tuple.address;
                    return true;
                }
            }

            return false;
        }
    }
}

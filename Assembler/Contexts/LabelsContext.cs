using Assembler._Infrastructure;
using Assembler.Readers;
using Assembler.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assembler.Contexts {
    class LabelsContext {
        public const string GlobalRegion = "@global";
        public const string ConstRegion = "@const";
        public const string CodeRegion = "@code";

        private static readonly string[] reservedRegions = { GlobalRegion, ConstRegion, CodeRegion };

        private class LabelInfo {
            public LabelInfo(string name, ushort? address) {
                Name = name;
                Address = address;
            }

            public string Name { get; set; }
            public ushort? Address { get; set; }
        }

        private class TokenInfo {
            public TokenInfo(string name, IToken value) {
                Name = name;
                Value = value;
            }

            public string Name { get; set; }
            public IToken Value { get; set; }
        }

        private readonly Dictionary<string, List<LabelInfo>> regionedLabels;
        private readonly Dictionary<string, List<TokenInfo>> regionedTokens;

        public string CurrentRegion { get; private set; } = GlobalRegion;
        public string CurrentReservedRegion { get; private set; }

        public void SetCurrentRegion(string region) {
            if (region.StartsWith('@')) {
                CurrentRegion = region.ToLower();
                CurrentReservedRegion = region.ToLower();
            } else {
                CurrentRegion = region;
            }
        }

        public LabelsContext() {
            regionedLabels = new Dictionary<string, List<LabelInfo>>();
            regionedTokens = new Dictionary<string, List<TokenInfo>>();
            ClearRegionedLabelsAndTokens();
        }

        public bool TryParseAllRegionsAndLabels(TokenReader reader, out bool isConstRegionDefined, out bool isCodeRegionDefined, out LabelToken mainLabel, out string errorMessage) {
            errorMessage = null;
            isConstRegionDefined = false;
            isCodeRegionDefined = false;
            mainLabel = null;

            string currentRegion = GlobalRegion;
            string currentLabel = null;
            bool isConstRegion = false;

            ClearRegionedLabelsAndTokens();

            do {
                if (reader.Current is RegionToken regionToken) {
                    bool isReservedRegion = reservedRegions.Contains(regionToken.Value.ToLower());
                    if (isReservedRegion) {
                        var regionTokenLower = regionToken.Value.ToLower();

                        if (regionTokenLower == GlobalRegion) {
                            errorMessage = $"Region {GlobalRegion} is reserved for compiler only and cannot be used";
                            return false;
                        } else if (regionTokenLower == ConstRegion) {
                            if (isCodeRegionDefined) {
                                errorMessage = "Const region must be defined before code region";
                                return false;
                            }
                            isConstRegion = true;
                            isConstRegionDefined = true;
                        } else if (regionTokenLower == CodeRegion) {
                            isConstRegion = false;
                            isCodeRegionDefined = true;
                        } else throw new NotImplementedException();

                        // continue;
                    } else {
                        if (regionToken.Value.StartsWith('@')) {
                            errorMessage = $"Only reserved regions can start with '@'. Reserved regions are: {string.Join(", ", reservedRegions)}";
                        }
                    }

                    /*if (isConstRegion) {
                        errorMessage = $"Cannot declare regions in reserved {ConstRegion} region";
                        return false;
                    }*/

                    /*if (!isReservedRegion && currentLabel == null) {
                        errorMessage = $"Region: {currentRegion} does not contain any label";
                        return false;
                    }*/

                    if (regionedLabels.ContainsKey(regionToken.Value)) {
                        errorMessage = $"Duplicated region: {regionToken.Value}";
                        return false;
                    }

                    regionedLabels.Add(regionToken.Value, new List<LabelInfo>());
                    currentRegion = regionToken.Value;
                    currentLabel = null;
                } else if (reader.Current is LabelToken labelToken) {
                    if (isConstRegion) {
                        errorMessage = $"Cannot declare labels in reserved {ConstRegion} region";
                        return false;
                    }

                    var prevLabels = regionedLabels[currentRegion];
                    if (prevLabels.Select(l => l.Name).Contains(labelToken.Value)) {
                        errorMessage = $"Duplicated label: {labelToken.Value} in region {currentRegion}";
                        return false;
                    }

                    prevLabels.Add(new LabelInfo(labelToken.Value, null));
                    currentLabel = labelToken.Value;

                    if (currentRegion.ToLower() == CodeRegion && mainLabel == null) {
                        mainLabel = labelToken;
                    }

                    if (!reader.Read() || reader.Current is not IdentifierToken identifierToken || (!identifierToken.IsInstruction(out _) && !identifierToken.IsPseudoinstruction(out _))) {
                        errorMessage = $"Instruction or pseudoinstruction expected after the Label {labelToken.Value}, got: {reader.Current.Class}";
                        return false;
                    }
                }
            } while (reader.Read());

            return true;
        }

        internal bool TryInsertRegionedToken(string name, IToken token, out string errorMessage) {
            errorMessage = null;

            var newTokenInfo = new TokenInfo(name, token);

            if (regionedTokens.ContainsKey(CurrentRegion)) {
                if (regionedTokens[CurrentRegion].Select(x => x.Name).Contains(name)) {
                    errorMessage = $"Duplicated identifier: {name} in region {CurrentRegion}";
                    return false;
                }

                regionedTokens[CurrentRegion].Add(newTokenInfo);
            } else {
                regionedTokens.Add(CurrentRegion, new List<TokenInfo> { newTokenInfo });
            }
            return true;
        }

        public bool TryFindLabel(string identifier, out ushort? address) {
            address = null;

            if (!TryGetRegionAndName(identifier, out var region, out var label)) {
                return false;
            }

            if (regionedLabels.TryGetValue(region, out var labels)) {
                var labelInfo = labels.FirstOrDefault(l => l.Name == label);
                if (labelInfo != null) {
                    address = labelInfo.Address;
                    return true;
                }
            }

            return false;
        }

        public bool TryFindRegionedToken(string name, out IToken token) {
            token = null;

            if (!TryGetRegionAndName(name, out var region, out var label)) {
                return false;
            }

            if (regionedTokens.TryGetValue(region, out var tokenInfos)) {
                var tokenInfo = tokenInfos.FirstOrDefault(l => l.Name == label);
                if (tokenInfo != null) {
                    token = tokenInfo.Value.DeepCopy();
                    return true;
                }
            }

            return false;
        }

        public bool TryResolveInvalidTokenException(InvalidTokenClassException ex, out IToken token) {
            token = null;

            if (ex.RecievedToken is not IdentifierToken identifierToken) {
                return false;
            }

            if (TryFindRegionedToken(identifierToken.Value, out var tmpToken)) {
                if (ex.ExpectedTokenClass.HasFlag(tmpToken.Class)) {
                    token = tmpToken;
                    return true;
                }
            }

            return false;
        }

        public bool TryResolveInvalidToken<TExpectedToken>(IToken recievedToken, out TExpectedToken token) where TExpectedToken : IToken {
            token = default;

            if (recievedToken is not IdentifierToken identifierToken) {
                return false;
            }

            if (TryFindRegionedToken(identifierToken.Value, out var tmpToken)) {
                if (tmpToken is TExpectedToken successToken) {
                    token = successToken;
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

        private bool TryGetRegionAndName(string identifier, out string region, out string label) {
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

        private void ClearRegionedLabelsAndTokens() {
            regionedLabels.Clear();
            regionedTokens.Clear();

            regionedLabels.Add(GlobalRegion, new List<LabelInfo>());
            regionedTokens.Add(GlobalRegion, new List<TokenInfo>());
        }
    }
}

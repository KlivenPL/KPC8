using Assembler._Infrastructure;
using Assembler.Contexts.Regions;
using Assembler.Parsers;
using Assembler.Readers;
using Assembler.Tokens;
using System;
using System.Linq;

namespace Assembler.Contexts.Labels {
    partial class LabelsContext {
        private readonly RegionParser regionParser;

        private ModuleRegion[] modules;
        private ModuleRegion currentModule;

        public LabelsContext(RegionParser regionParser) {
            this.regionParser = regionParser;
            currentModule = null;
            CurrentRegion = null;
        }

        public IRegion CurrentRegion { get; private set; }

        public bool TryPreParseRegions(TokenReader tokenReader, out string mainLabelIdentifier, out string errorMessage) {
            mainLabelIdentifier = null;
            errorMessage = null;

            try {
                regionParser.PreParseAllRegions(tokenReader, out mainLabelIdentifier, out var constRegion, out var modules);
                this.modules = modules.ToArray();
                CurrentRegion = constRegion;
                currentModule = modules[0];
                return true;
            } catch (OtherInnerException ex) {
                errorMessage = ex.Message;
                return false;
            }
        }

        public void SetCurrentModule(IdentifierToken moduleNameToken) {
            currentModule = modules.First(x => x.Name == moduleNameToken.Value);
            CurrentRegion = currentModule;
        }

        public void SetCurrentRegion(RegionToken regionToken) {
            CurrentRegion = currentModule.GetRegion(regionToken.Value);
        }

        internal bool TryInsertRegionedToken(string name, IToken token, out string errorMessage) {
            try {
                CurrentRegion.InsertToken(name, token);
            } catch (OtherInnerException ex) {
                errorMessage = ex.Message;
                return false;
            }

            errorMessage = null;
            return true;
        }

        public bool TryFindLabel(string identifier, out ushort? address) {
            address = null;

            if (!TryLocalizeRegion(identifier, out var region, out var identifierStr)) {
                return false;
            }

            try {
                var labelInfo = region.GetLabel(identifierStr);
                address = labelInfo.Address;
            } catch (OtherInnerException) {
                return false;
            }

            return true;
        }

        public bool TryFindToken(string name, out IToken token) {
            token = null;

            if (!TryLocalizeRegion(name, out var region, out var identifierStr)) {
                return false;
            }

            try {
                var tokenInfo = region.GetToken(identifierStr);
                token = tokenInfo.Value.DeepCopy();
            } catch (OtherInnerException) {
                return false;
            }

            return true;
        }

        public bool TryResolveInvalidTokenException(InvalidTokenClassException ex, out IToken token) {
            token = null;

            if (ex.RecievedToken is not IdentifierToken identifierToken) {
                return false;
            }

            if (TryFindToken(identifierToken.Value, out var tmpToken)) {
                if (ex.ExpectedTokenClass.HasFlag(tmpToken.Class)) {
                    token = tmpToken;
                    return true;
                }
            }

            return false;
        }

        public bool TryResolveLabelNotResolvedException(LabelNotResolvedException ex, out ushort address) {
            address = 0;

            IRegion region = ex.Region;
            string identifierStr = ex.ArgumentToken.Value;

            if (identifierStr.Contains('.')) {
                if (TryLocalizeRegion(identifierStr, out var tmpRegion, out var labelIdentifier, exportedOnly: false)) {
                    region = tmpRegion;
                    identifierStr = labelIdentifier;
                }
            }

            try {
                address = region.GetLabel(identifierStr).Address ?? throw new Exception("Label not resolved");
            } catch (OtherInnerException) {
                return false;
            } catch (Exception) {
                return false;
            }

            return true;
        }

        public bool TryResolveInvalidToken<TExpectedToken>(IToken recievedToken, out TExpectedToken token) where TExpectedToken : IToken {
            token = default;

            if (recievedToken is not IdentifierToken identifierToken) {
                return false;
            }

            if (TryFindToken(identifierToken.Value, out var tmpToken)) {
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

            CurrentRegion.GetLabel(labelDef).Address = address;
        }

        private bool TryLocalizeRegion(string identifier, out IRegion region, out string identifierStr, bool? exportedOnly = null) {
            var split = identifier.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            region = CurrentRegion;

            string moduleStr = currentModule.Name;
            string regionStr = CurrentRegion.Name;
            identifierStr = null;

            if (split.Length == 1) {
                identifierStr = split[0];
            } else if (split.Length == 2) {
                regionStr = split[0];
                identifierStr = split[1];
            } else if (split.Length == 3) {
                moduleStr = split[0];
                regionStr = split[1];
                identifierStr = split[2];
            } else {
                return false;
            }

            exportedOnly ??= moduleStr != currentModule.Name;

            try {
                var module = modules.FirstOrDefault(x => x.Name == moduleStr) ?? throw new Exception($"Module {moduleStr} not found");
                region = exportedOnly == true ? module.GetExportedRegion(regionStr) : module.GetRegion(regionStr);
            } catch (OtherInnerException) {
                return false;
            } catch (Exception) {
                return false;
            }

            return true;
        }
    }
}

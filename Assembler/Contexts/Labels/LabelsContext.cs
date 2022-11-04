using Assembler._Infrastructure;
using Assembler.Contexts.Regions;
using Assembler.Parsers;
using Assembler.Readers;
using Assembler.Tokens;
using System;
using System.Linq;

namespace Assembler.Contexts.Labels {
    partial class LabelsContext {
        private readonly RegionPreParser regionParser;

        private ModuleRegion[] modules;

        public LabelsContext(RegionPreParser regionParser) {
            this.regionParser = regionParser;
            CurrentModule = null;
            CurrentRegion = null;
        }

        public IRegion CurrentRegion { get; private set; }
        internal ModuleRegion CurrentModule { get; set; }

        public bool TryPreParseRegions(TokenReader tokenReader, ConstRegion constRegion, out string mainLabelIdentifier, out string errorMessage) {
            mainLabelIdentifier = null;
            errorMessage = null;

            try {
                regionParser.PreParseAllRegions(tokenReader, constRegion, out mainLabelIdentifier, out var modules);
                this.modules = modules.ToArray();
                CurrentRegion = constRegion;
                CurrentModule = modules[0];
                return true;
            } catch (OtherInnerException ex) {
                errorMessage = ex.Message;
                return false;
            }
        }

        public void SetCurrentModule(IdentifierToken moduleNameToken) {
            CurrentModule = modules.First(x => x.Name == moduleNameToken.Value);
            CurrentRegion = CurrentModule;
        }

        public void SetCurrentRegion(RegionToken regionToken) {
            CurrentRegion = CurrentModule.GetRegion(regionToken.Value);
        }

        public bool TryInsertToken(string name, IToken token, out string errorMessage) {
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

        public bool TryResolveLabelNotResolvedException(LabelNotResolvedException ex, out ushort address) {
            address = 0;

            // only for first jl in ROM (main label)
            if ((ex.Region is ConstRegion) && ex.ArgumentToken.Value.Contains('.') && TryLocalizeRegion(ex.ArgumentToken.Value, out var tmpRegion, out var labelIdentifier, exportedOnly: false)) {
                address = tmpRegion.GetLabel(labelIdentifier).Address ?? throw new Exception("Main label not resolved. This should not have happened");
                return true;
            }

            SplitIdentifier(ex.ArgumentToken.Value, out var moduleStr, out var regionStr, out var identifierStr);

            try {
                address = TryAllThenThrow(GetFromLocalRegion, GetFromOtherRegion, GetFromOtherModule);
                return true;
            } catch (Exception) {
                return false;
            }

            return false;

            ushort GetFromLocalRegion() {
                if (moduleStr != null || regionStr != null) {
                    throw new OtherInnerException("Go for other region");
                }

                return ex.Region.GetLabel(identifierStr)?.Address.Value ?? throw new Exception();
            }

            ushort GetFromOtherRegion() {
                if (moduleStr != null) {
                    throw new OtherInnerException("Go for other module");
                }

                return ex.Module.GetRegion(regionStr)?.GetLabel(identifierStr)?.Address ?? throw new Exception();
            }

            ushort GetFromOtherModule() {
                return modules.FirstOrDefault(x => x.Name == moduleStr)?.GetExportedRegion(regionStr).GetLabel(identifierStr)?.Address ?? throw new Exception();
            }
        }

        public bool TryResolveInvalidTokenException(InvalidTokenClassException ex, out IToken token) {
            token = null;

            if (ex.RecievedToken is not IdentifierToken identifierToken) {
                return false;
            }

            SplitIdentifier(identifierToken.Value, out var moduleStr, out var regionStr, out var identifierStr);

            try {
                token = TryAllThenThrow(GetFromLocalRegion, GetFromOtherRegion, GetFromOtherModule).DeepCopy();
                return true;
            } catch (Exception) {
                return false;
            }

            return false;

            IToken GetFromLocalRegion() {
                if (moduleStr != null || regionStr != null) {
                    throw new OtherInnerException("Go for other region");
                }

                return ex.Region.GetToken(identifierStr)?.Value ?? throw new Exception();
            }

            IToken GetFromOtherRegion() {
                if (moduleStr != null) {
                    throw new OtherInnerException("Go for other module");
                }

                return ex.Module.GetRegion(regionStr)?.GetToken(identifierStr)?.Value ?? throw new Exception();
            }

            IToken GetFromOtherModule() {
                return modules.FirstOrDefault(x => x.Name == moduleStr)?.GetExportedRegion(regionStr).GetToken(identifierStr)?.Value ?? throw new Exception();
            }
        }

        public bool TryResolveInvalidToken<TExpectedToken>(IToken recievedToken, out TExpectedToken token) where TExpectedToken : IToken {
            token = default;

            if (recievedToken is not IdentifierToken identifierToken) {
                return false;
            }

            SplitIdentifier(identifierToken.Value, out var moduleStr, out var regionStr, out var identifierStr);

            try {
                if (TryAllThenThrow(GetFromLocalRegion, GetFromOtherRegion, GetFromOtherModule).DeepCopy() is TExpectedToken expectedToken) {
                    token = expectedToken;
                    return true;
                }
            } catch (Exception) {
                return false;
            }

            return false;

            IToken GetFromLocalRegion() {
                if (moduleStr != null || regionStr != null) {
                    throw new OtherInnerException("Go for other region");
                }

                return CurrentRegion.GetToken(identifierStr)?.Value ?? throw new Exception();
            }

            IToken GetFromOtherRegion() {
                if (moduleStr != null) {
                    throw new OtherInnerException("Go for other module");
                }

                return CurrentModule.GetRegion(regionStr)?.GetToken(identifierStr)?.Value ?? throw new Exception();
            }

            IToken GetFromOtherModule() {
                return modules.FirstOrDefault(x => x.Name == moduleStr)?.GetExportedRegion(regionStr).GetToken(identifierStr)?.Value ?? throw new Exception();
            }
        }

        public void ResolveLabel(string labelDef, ushort address) {
            if (!TryFindLabel(labelDef, out var oldAddress)) {
                throw new Exception("Label not found");
            } else if (oldAddress.HasValue) {
                throw new Exception("Label already resolved");
            }

            CurrentRegion.GetLabel(labelDef).Address = address;
        }

        public void ResetCurrentModuleAndRegion() {
            CurrentModule = null;
            CurrentRegion = null;
        }

        private bool TryLocalizeRegion(string identifier, out IRegion region, out string identifierStr, bool? exportedOnly = null) {
            var split = identifier.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            region = CurrentRegion;

            string moduleStr = CurrentModule?.Name;
            string regionStr = CurrentRegion?.Name;
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

            exportedOnly ??= moduleStr != CurrentModule.Name;

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

        private static void SplitIdentifier(string identifier, out string moduleStr, out string regionStr, out string identifierStr) {
            moduleStr = null;
            regionStr = null;
            identifierStr = null;

            var split = identifier.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 1) {
                identifierStr = split[0];
            } else if (split.Length == 2) {
                regionStr = split[0];
                identifierStr = split[1];
            } else if (split.Length == 3) {
                moduleStr = split[0];
                regionStr = split[1];
                identifierStr = split[2];
            }
        }

        private static T TryAllThenThrow<T>(params Func<T>[] triees) {
            foreach (var triee in triees) {
                try {
                    return triee();
                } catch (OtherInnerException) {
                } catch (Exception) {
                    throw;
                }
            }

            throw new Exception();
        }
    }
}

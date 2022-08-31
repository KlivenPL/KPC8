using Assembler._Infrastructure;
using Assembler.Builders;
using Assembler.Contexts.Labels;
using Assembler.Contexts.Regions;
using Assembler.DebugData;
using Assembler.Encoders;
using Assembler.Readers;
using Assembler.Tokens;
using System;
using System.Collections.Generic;

namespace Assembler.Commands {
    internal abstract class CommandBase {
        public abstract CommandType Type { get; }
        protected delegate bool TryInsertTokenDelegate(string name, IToken token, out string errorMessage);
        private IRegion currentPreParseRegion;

        public void Parse(TokenReader reader, LabelsContext labelsContext, RomBuilder romBuilder, List<IDebugSymbol> debugSymbols) {
            ValidateRegions(reader, labelsContext.CurrentRegion);
            AddConstantDebugSymbol = debugSymbols.Add;
            AddDebugWriteSymbol = debugSymbols.Add;
            TryInsertToken = labelsContext.TryInsertToken;
            currentPreParseRegion = null;
            ParseInner(reader, labelsContext, romBuilder);
        }

        public void PreParse(TokenReader reader, IRegion currentRegion) {
            ValidateRegions(reader, currentRegion);
            AddConstantDebugSymbol = null;
            AddDebugWriteSymbol = null;
            currentPreParseRegion = currentRegion;
            TryInsertToken = TryInsertTokenPreParse;
            PreParseInner(reader, currentRegion);
        }

        private bool TryInsertTokenPreParse(string name, IToken token, out string errorMessage) {
            try {
                currentPreParseRegion.InsertToken(name, token);
            } catch (OtherInnerException ex) {
                errorMessage = ex.Message;
                return false;
            }

            errorMessage = null;
            return true;
        }

        protected abstract CommandAllowedIn AcceptedRegions { get; }
        protected abstract void ParseInner(TokenReader reader, LabelsContext labelsContext, RomBuilder romBuilder);
        protected virtual void PreParseInner(TokenReader reader, IRegion region) => throw new NotImplementedException();
        protected InstructionEncoder InstructionEncoder { get; } = new InstructionEncoder();

        protected Action<ConstantValueSymbol> AddConstantDebugSymbol;
        protected Action<DebugWriteSymbol> AddDebugWriteSymbol;
        protected TryInsertTokenDelegate TryInsertToken;

        protected void ParseParameters<T>(TokenReader reader, out T parsedToken) where T : IToken {
            parsedToken = ParseNextParameter<T>(reader);
        }

        protected void ParseParameters<T1, T2>(TokenReader reader, out T1 parsedToken1, out T2 parsedToken2) where T1 : IToken where T2 : IToken {
            parsedToken1 = ParseNextParameter<T1>(reader);
            parsedToken2 = ParseNextParameter<T2>(reader);
        }

        protected void ParseParameters<T1, T2, T3>(TokenReader reader, out T1 parsedToken1, out T2 parsedToken2, out T3 parsedToken3) where T1 : IToken where T2 : IToken where T3 : IToken {
            parsedToken1 = ParseNextParameter<T1>(reader);
            parsedToken2 = ParseNextParameter<T2>(reader);
            parsedToken3 = ParseNextParameter<T3>(reader);
        }

        protected void ParseParameters<T1, T2, T3, T4>(TokenReader reader, out T1 parsedToken1, out T2 parsedToken2, out T3 parsedToken3, out T4 parsedToken4) where T1 : IToken where T2 : IToken where T3 : IToken where T4 : IToken {
            parsedToken1 = ParseNextParameter<T1>(reader);
            parsedToken2 = ParseNextParameter<T2>(reader);
            parsedToken3 = ParseNextParameter<T3>(reader);
            parsedToken4 = ParseNextParameter<T4>(reader);
        }

        protected void SplitWord(ushort word, out byte lower, out byte higher) {
            lower = (byte)(word & 0x00FF);
            higher = (byte)((word >> 8) & 0x00FF);
        }

        protected virtual void ValidateRegions(TokenReader reader, IRegion currentRegion) {
            if (AcceptedRegions == CommandAllowedIn.None) {
                throw new System.Exception("Commands must define accepted regions");
            }

            if (!CheckIsRegionAllowed(currentRegion, AcceptedRegions)) {
                throw ParserException.Create($"Command {Type} can only be used in following reserved regions: {string.Join(", ", AcceptedRegions)}", reader.Current);
            }
        }

        private bool CheckIsRegionAllowed(IRegion currentRegion, CommandAllowedIn allowIn) {
            if (currentRegion.IsReserved) {
                if (currentRegion is ModuleRegion) {
                    return allowIn.HasFlag(CommandAllowedIn.ModuleRegion);
                }

                if (currentRegion is ConstRegion) {
                    return allowIn.HasFlag(CommandAllowedIn.ConstRegion);
                }

            } else {
                return allowIn.HasFlag(CommandAllowedIn.UserDefinedRegion);
            }

            throw new NotImplementedException("Something went wrong on checking allowed command regions");
        }

        private T ParseNextParameter<T>(TokenReader reader) where T : IToken {
            if (reader.Read() && reader.Current is T parsed) {
                return parsed;
            } else {
                throw ParserException.Create($"Invalid {Type} command parameter. Expected class {typeof(T).Name}, got {reader.Current.Class}", reader.Current);
            }
        }

        [Flags]
        protected enum CommandAllowedIn {
            None = 0,
            ConstRegion = 1,
            ModuleRegion = 2,
            UserDefinedRegion = 4,
            Everywhere = 1 | 2 | 4,
        }
    }
}

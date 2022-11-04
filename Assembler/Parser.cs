using Assembler._Infrastructure;
using Assembler.Builders;
using Assembler.Contexts;
using Assembler.Contexts.Labels;
using Assembler.Contexts.Regions;
using Assembler.DebugData;
using Assembler.Encoders;
using Assembler.Parsers;
using Assembler.Readers;
using Assembler.Tokens;
using System.Collections;
using System.Collections.Generic;

namespace Assembler {
    public class Parser {

        private readonly LabelsContext labelsContext;
        private readonly RegionPreParser regionParser;
        private readonly InstructionParser instructionParser;
        private readonly CommandParser commandParser;
        private readonly PseudoinstructionParser pseudoinstructionParser;
        private readonly List<LabelNotResolvedException> unresolvedPseudoinstructions;
        private readonly InstructionEncoder instructionEncoder;

        public Parser() {
            regionParser = new RegionPreParser();
            labelsContext = new LabelsContext(regionParser);
            instructionEncoder = new InstructionEncoder();
            instructionParser = new InstructionParser(new InstructionsContext(), instructionEncoder, labelsContext);
            pseudoinstructionParser = new PseudoinstructionParser(new PseudoinstructionsContext(labelsContext));
            commandParser = new CommandParser(new CommandsContext(), labelsContext);
            unresolvedPseudoinstructions = new List<LabelNotResolvedException>();
        }

        public BitArray[] Parse(TokenReader reader) => Parse(reader, out _);

        public BitArray[] Parse(TokenReader reader, out IEnumerable<IDebugSymbol> debugSymbols) {
            var romBuilder = new RomBuilder();
            var debugSymbolList = new List<IDebugSymbol>();
            LabelToken lastUnresolvedLabelToken = null;

            reader.Read();

            InsertModules(ref reader, out var constRegion);

            PreParseAllRegions(ref reader, constRegion);

            ParseTokens(ref reader, romBuilder, debugSymbolList, ref lastUnresolvedLabelToken);

            labelsContext.ResetCurrentModuleAndRegion();

            ResolvePseudoinstructions(romBuilder, debugSymbolList);

            debugSymbols = debugSymbolList;
            return romBuilder.Build();
        }

        private void InsertModules(ref TokenReader reader, out ConstRegion constRegion) {
            constRegion = regionParser.PreParseConstRegion(reader);
            var origPos = reader.Position;
            var tokens = reader.GetTokens();
            tokens.AddRange(constRegion.InsertedModuleTokens);
            reader = new TokenReader(tokens, origPos);
        }

        private void ResolvePseudoinstructions(RomBuilder romBuilder, List<IDebugSymbol> debugSymbolList) {
            foreach (var unresolvedPseudoinstruction in unresolvedPseudoinstructions) {
                if (!labelsContext.TryResolveLabelNotResolvedException(unresolvedPseudoinstruction, out var address)) {
                    throw ParserException.Create($"Unresolved label identifier: {unresolvedPseudoinstruction.ArgumentToken}", unresolvedPseudoinstruction.ArgumentToken);
                }

                var pseudoinstruction = unresolvedPseudoinstruction.Resolve(address);
                romBuilder.NextAddress = unresolvedPseudoinstruction.Address;
                romBuilder.Unreserve(unresolvedPseudoinstruction.SizeInBytes);
                romBuilder.AddPseudoinstruction(pseudoinstruction, out var loAddress);
                debugSymbolList.Add(new ExecutableSymbol(unresolvedPseudoinstruction.PseudoinstructionToken, loAddress));
            }
        }

        private void ParseTokens(ref TokenReader reader, RomBuilder romBuilder, List<IDebugSymbol> debugSymbolList, ref LabelToken lastUnresolvedLabelToken) {
            var origReader = reader.Clone();
            while (reader.Read()) {
                switch (reader.Current.Class) {
                    case TokenClass.Identifier:
                        // obejscie pierwszego jl do main label (romBuilder.NextAddress != 0)
                        if (romBuilder.NextAddress != 0 && labelsContext.CurrentRegion.Name == RegionPreParser.ConstRegionName) {
                            throw ParserException.Create($"Identifiers are not allowed in reserved region {RegionPreParser.ConstRegionName}", reader.Current);
                        }
                        reader = ParseIdentifier(reader, romBuilder, debugSymbolList, ref lastUnresolvedLabelToken);
                        break;
                    case TokenClass.Label:
                        lastUnresolvedLabelToken = reader.CastCurrent<LabelToken>();
                        break;
                    case TokenClass.Region:
                        var regionToken = reader.CastCurrent<RegionToken>();
                        if (regionToken.Value.ToLower() == RegionPreParser.ModuleRegionName) {
                            reader.Read();
                            var moduleNameToken = reader.CastCurrent<IdentifierToken>();
                            labelsContext.SetCurrentModule(moduleNameToken);
                        } else {
                            labelsContext.SetCurrentRegion(regionToken);
                        }
                        break;
                    case TokenClass.Command:
                        commandParser.Parse(reader, romBuilder, debugSymbolList);
                        break;
                    default:
                        throw ParserException.Create($"Unexpected token: {reader.Current} of class: {reader.Current.Class}", reader.Current);
                }
            }
            reader = origReader;
        }

        private void PreParseAllRegions(ref TokenReader reader, ConstRegion constRegion) {
            //  var clonedReader = reader.Clone();

            if (!labelsContext.TryPreParseRegions(reader, constRegion, out var mainLabelIdentifier, out var errorMessage)) {
                throw ParserException.Create($"Error while parsing regions: {errorMessage}", reader.Current);
            }

            // var tokens = clonedReader.GetTokens();
            var tokens = reader.GetTokens();
            var jlToken = new IdentifierToken("jl", 0, -1, null);
            var mainIdentifier = new IdentifierToken(mainLabelIdentifier, 0, -1, null);
            tokens.InsertRange(0, new IToken[] { jlToken, mainIdentifier });
            reader = new TokenReader(tokens);
        }

        private TokenReader ParseIdentifier(TokenReader reader, RomBuilder romBuilder, List<IDebugSymbol> debugSymbolList, ref LabelToken lastUnresolvedLabelToken) {
            var readerClone = reader.Clone();

            if (lastUnresolvedLabelToken != null) {
                labelsContext.ResolveLabel(lastUnresolvedLabelToken.Value, romBuilder.NextAddress);
                lastUnresolvedLabelToken = null;
            }

            var identifier = reader.CastCurrent<IdentifierToken>();

            try {
                instructionParser.Parse(reader, out var instructionHigh, out var instructionLow);
                romBuilder.AddInstruction(instructionHigh, instructionLow, out var loAddress);
                debugSymbolList.Add(new ExecutableSymbol(identifier, loAddress));
            } catch (RegisterChangedToAssException ex) {
                instructionEncoder.Encode(KPC8.RomProgrammers.Microcode.McInstructionType.Setw, KPC8.ProgRegs.Regs.Zero, KPC8.ProgRegs.Regs.Ass, ex.ChangedToken.Value, out var preAssInstrHigh, out var preAssInstrLow);
                romBuilder.AddInstruction(preAssInstrHigh, preAssInstrLow, out var preAssLoAddress);

                romBuilder.AddInstruction(ex.InstructionHigh, ex.InstructionLow, out var loAddress);
                debugSymbolList.Add(new ExecutableSymbol(identifier, (ushort)(loAddress + 2)));

                var tokens = reader.GetTokens();
                tokens.InsertRange(reader.Position + 1, new IToken[] {
                    new IdentifierToken("setw", -1, -1, null),
                    new RegisterToken(ex.ChangedToken.Value, -1, -1, null),
                    new RegisterToken(KPC8.ProgRegs.Regs.Ass, -1, -1, null),
                });

                var pos = reader.Position;
                reader = new TokenReader(tokens);
                reader.MoveTo(pos);

            } catch (ParserException) {
                if (readerClone.CastCurrent<IdentifierToken>().IsPseudoinstruction(out _)) {
                    IdentifierToken pseudoinstructionToken = null;
                    try {
                        pseudoinstructionToken = reader.CastCurrent<IdentifierToken>();

                    } catch {
                        throw;
                    }
                    try {
                        var pseudoinstruction = pseudoinstructionParser.Parse(readerClone);
                        romBuilder.AddPseudoinstruction(pseudoinstruction, out var loAddress);
                        debugSymbolList.Add(new ExecutableSymbol(pseudoinstructionToken, loAddress));
                    } catch (LabelNotResolvedException labelNotResolvedException) {
                        romBuilder.Reserve(labelNotResolvedException.SizeInBytes);
                        labelNotResolvedException.SetAddress(romBuilder.NextAddress);
                        labelNotResolvedException.SetPseudoinstructionToken(pseudoinstructionToken);
                        romBuilder.NextAddress += labelNotResolvedException.SizeInBytes;
                        unresolvedPseudoinstructions.Add(labelNotResolvedException);
                    } finally {
                        reader = readerClone;
                    }
                } else {
                    throw;
                }
            }

            return reader;
        }
    }
}

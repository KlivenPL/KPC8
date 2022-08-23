using Assembler._Infrastructure;
using Assembler.Builders;
using Assembler.Contexts;
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
        private readonly InstructionParser instructionParser;
        private readonly CommandParser commandParser;
        private readonly PseudoinstructionParser pseudoinstructionParser;
        private readonly List<LabelNotResolvedException> unresolvedPseudoinstructions;
        private readonly InstructionEncoder instructionEncoder;

        public Parser() {
            labelsContext = new LabelsContext();
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

            {
                var clonedReader = reader.Clone();
                if (!labelsContext.TryParseAllRegionsAndLabels(clonedReader, out var isConstRegionDefined, out var isCodeRegionDefined, out var mainLabel, out var errorMessage)) {
                    throw ParserException.Create($"Error while parsing labels: {errorMessage}", clonedReader.Current);
                }

                if (isConstRegionDefined && isCodeRegionDefined) {
                    if (mainLabel == null) {
                        throw ParserException.Create($"No entry point found. Define \":main\" label in {LabelsContext.CodeRegion} region", clonedReader.Current);
                    }

                    var tokens = clonedReader.GetTokens();
                    var jlToken = new IdentifierToken("jl", 0, -1);
                    var mainIdentifier = new IdentifierToken($"{LabelsContext.CodeRegion}.{mainLabel.Value}", 0, -1);
                    tokens.InsertRange(0, new IToken[] { jlToken, mainIdentifier });
                    reader = new TokenReader(tokens);
                }
            }

            var origReader = reader.Clone();
            while (reader.Read()) {
                switch (reader.Current.Class) {
                    case TokenClass.Identifier:
                        if (labelsContext.CurrentReservedRegion == LabelsContext.ConstRegion) {
                            throw ParserException.Create($"Identifiers are not allowed in reserved region {LabelsContext.ConstRegion}", reader.Current);
                        }
                        reader = ParseIdentifier(reader, romBuilder, debugSymbolList, ref lastUnresolvedLabelToken);
                        break;
                    case TokenClass.Label:
                        lastUnresolvedLabelToken = reader.CastCurrent<LabelToken>();
                        break;
                    case TokenClass.Region:
                        labelsContext.SetCurrentRegion(reader.CastCurrent<RegionToken>().Value);
                        break;
                    case TokenClass.Command:
                        commandParser.Parse(reader, romBuilder, debugSymbolList);
                        break;
                    default:
                        throw ParserException.Create($"Unexpected token: {reader.Current} of class: {reader.Current.Class}", reader.Current);
                }
            }
            reader = origReader;

            foreach (var unresolvedPseudoinstruction in unresolvedPseudoinstructions) {
                var label = unresolvedPseudoinstruction.ArgumentToken.Value.Contains('.') ?
                    unresolvedPseudoinstruction.ArgumentToken.Value : $"{unresolvedPseudoinstruction.Region}.{unresolvedPseudoinstruction.ArgumentToken.Value}";

                if (!labelsContext.TryFindLabel(label, out var address) || !address.HasValue) {
                    throw ParserException.Create($"Unresolved label identifier: {unresolvedPseudoinstruction.ArgumentToken}", unresolvedPseudoinstruction.ArgumentToken);
                }

                var pseudoinstruction = unresolvedPseudoinstruction.Resolve(address.Value);
                romBuilder.NextAddress = unresolvedPseudoinstruction.Address;
                romBuilder.Unreserve(unresolvedPseudoinstruction.SizeInBytes);
                romBuilder.AddPseudoinstruction(pseudoinstruction, out var loAddress);
                debugSymbolList.Add(new ExecutableSymbol(unresolvedPseudoinstruction.PseudoinstructionToken, loAddress));
            }

            debugSymbols = debugSymbolList;
            return romBuilder.Build();
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
                    new IdentifierToken("setw", -1, -1),
                    new RegisterToken(ex.ChangedToken.Value, -1, -1),
                    new RegisterToken(KPC8.ProgRegs.Regs.Ass, -1, -1),
                });

                var pos = reader.Position;
                reader = new TokenReader(tokens);
                reader.MoveTo(pos);

            } catch (ParserException) {
                if (readerClone.CastCurrent<IdentifierToken>().IsPseudoinstruction(out _)) {
                    var pseudoinstructionToken = reader.CastCurrent<IdentifierToken>();
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

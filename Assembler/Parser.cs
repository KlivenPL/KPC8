using Assembler._Infrastructure;
using Assembler.Builders;
using Assembler.Contexts;
using Assembler.DebugData;
using Assembler.Encoders;
using Assembler.Parsers;
using Assembler.Readers;
using Assembler.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Assembler {
    public class Parser {

        private readonly LabelsContext labelsContext;
        private readonly InstructionParser instructionParser;
        private readonly PseudoinstructionParser pseudoinstructionParser;
        private readonly List<LabelNotResolvedException> unresolvedPseudoinstructions;

        public Parser() {
            labelsContext = new LabelsContext();
            instructionParser = new InstructionParser(new InstructionsContext(), new InstructionEncoder());
            pseudoinstructionParser = new PseudoinstructionParser(new PseudoinstructionsContext());
            unresolvedPseudoinstructions = new List<LabelNotResolvedException>();
        }

        public BitArray[] Parse(TokenReader reader) => Parse(reader, out _);

        public BitArray[] Parse(TokenReader reader, out IEnumerable<IDebugSymbol> debugSymbols) {
            var origReader = reader.Clone();
            var romBuilder = new RomBuilder();
            var debugSymbolList = new List<IDebugSymbol>();
            LabelToken lastUnresolvedLabelToken = null;

            {
                var clonedReader = reader.Clone();
                if (!labelsContext.TryParseAllRegionsAndLabels(clonedReader, out var errorMessage)) {
                    throw ParserException.Create($"Error while parsing labels: {errorMessage}", clonedReader.Current);
                }
            }

            while (reader.Read()) {
                switch (reader.Current.Class) {
                    case TokenClass.Identifier:
                        reader = ParseIdentifier(reader, romBuilder, debugSymbolList, ref lastUnresolvedLabelToken);
                        break;
                    case TokenClass.Label:
                        lastUnresolvedLabelToken = reader.CastCurrent<LabelToken>();
                        break;
                    case TokenClass.Region:
                        labelsContext.CurrentRegion = reader.CastCurrent<RegionToken>().Value;
                        break;
                    case TokenClass.Command:
                        throw new NotImplementedException();
                    default:
                        throw ParserException.Create($"Unexpected token: {reader.Current} of class: {reader.Current.Class}", reader.Current);
                }
            }

            reader = origReader;
            foreach (var unresolvedPseudoinstruction in unresolvedPseudoinstructions) {
                if (!labelsContext.TryFindLabel(unresolvedPseudoinstruction.ArgumentToken.Value, out var address) || !address.HasValue) {
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

            try {
                var identifier = reader.CastCurrent<IdentifierToken>();
                instructionParser.Parse(reader, out var instructionHigh, out var instructionLow);
                romBuilder.AddInstruction(instructionHigh, instructionLow, out var loAddress);
                debugSymbolList.Add(new ExecutableSymbol(identifier, loAddress));
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

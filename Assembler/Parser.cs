using Assembler._Infrastructure;
using Assembler.Builders;
using Assembler.Contexts;
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

        public BitArray[] Parse(TokenReader reader) {
            var origReader = reader.Clone();
            var romBuilder = new RomBuilder();
            LabelToken lastUnresolvedLabelToken = null;

            if (!labelsContext.TryParseAllRegionsAndLabels(reader.Clone())) {
                throw ParserException.Create("Error while parsing labels", reader.Current);
            }

            while (reader.Read()) {
                switch (reader.Current.Class) {
                    case TokenClass.Identifier:
                        reader = ParseIdentifier(reader, romBuilder, ref lastUnresolvedLabelToken);
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
                if (!labelsContext.TryFindLabel(unresolvedPseudoinstruction.IdentifierToken.Value, out var address) || !address.HasValue) {
                    throw ParserException.Create($"Unresolved label identifier: {unresolvedPseudoinstruction.IdentifierToken}", unresolvedPseudoinstruction.IdentifierToken);
                }

                var pseudoinstruction = unresolvedPseudoinstruction.Resolve(address.Value);
                romBuilder.NextAddress = unresolvedPseudoinstruction.Address;
                romBuilder.Unreserve(unresolvedPseudoinstruction.SizeInBytes);
                romBuilder.AddPseudoinstruction(pseudoinstruction);
            }

            return romBuilder.Build();
        }

        private TokenReader ParseIdentifier(TokenReader reader, RomBuilder romBuilder, ref LabelToken lastUnresolvedLabelToken) {
            var readerClone = reader.Clone();

            if (lastUnresolvedLabelToken != null) {
                labelsContext.ResolveLabel(lastUnresolvedLabelToken.Value, romBuilder.NextAddress);
                lastUnresolvedLabelToken = null;
            }

            try {
                instructionParser.Parse(reader, out var instructionHigh, out var instructionLow);
                romBuilder.AddInstruction(instructionHigh, instructionLow);
            } catch (ParserException) {
                if (readerClone.CastCurrent<IdentifierToken>().IsPseudoinstruction(out _)) {
                    try {
                        var pseudoinstruction = pseudoinstructionParser.Parse(readerClone);
                        romBuilder.AddPseudoinstruction(pseudoinstruction);
                    } catch (LabelNotResolvedException labelNotResolvedException) {
                        romBuilder.Reserve(labelNotResolvedException.SizeInBytes);
                        labelNotResolvedException.SetAddress(romBuilder.NextAddress);
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

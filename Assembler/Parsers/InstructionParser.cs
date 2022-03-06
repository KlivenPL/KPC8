using Assembler._Infrastructure;
using Assembler.Contexts;
using Assembler.Encoders;
using Assembler.Readers;
using Assembler.Tokens;
using KPC8._Infrastructure.Microcode.Attributes;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System;
using System.Collections;

namespace Assembler.Parsers {
    class InstructionParser {
        private readonly InstructionEncoder instructionEncoder;
        private readonly InstructionsContext instructionsContext;

        public InstructionParser(InstructionsContext instructionsContext, InstructionEncoder instructionEncoder) {
            this.instructionsContext = instructionsContext;
            this.instructionEncoder = instructionEncoder;
        }

        public void Parse(TokenReader reader, out BitArray instructionHigh, out BitArray instructionLow) {
            instructionHigh = null;
            instructionLow = null;
            var identifier = reader.CastCurrent<IdentifierToken>();

            if (identifier.IsInstruction(out var instructionType)) {
                var instructionFormat = instructionsContext.GetInstructionFormat(instructionType);

                switch (instructionFormat.InstructionFormat) {
                    case McInstructionFormat.Register:
                        ParseRegisterInstruction(reader, out instructionHigh, out instructionLow, instructionType, instructionFormat);
                        break;
                    case McInstructionFormat.Immediate:
                        ParseImmediateInstruction(reader, out instructionHigh, out instructionLow, instructionType, instructionFormat);
                        break;
                    default:
                        throw new NotImplementedException();
                }

            } else {
                throw ParserException.Create($"Identifier is not an instruction", reader.Current);
            }
        }

        private void ParseImmediateInstruction(TokenReader reader, out BitArray instructionHigh, out BitArray instructionLow, McInstructionType instructionType, InstructionFormatAttribute instructionFormat) {
            Regs regDest;
            byte number;

            if (instructionFormat.RegDestRestrictions != InstructionFormatAttribute.DefaultRegDestRestrictions) {
                regDest = instructionFormat.RegDestRestrictions;
            } else {
                if (reader.Read() && reader.Current is RegisterToken registerToken) {
                    regDest = registerToken.Value;
                } else {
                    throw ParserException.Create($"Expected register, got {reader.Current.Class}", reader.Current);
                }
            }

            if (instructionFormat.ImmediateValue.HasValue) {
                number = (byte)instructionFormat.ImmediateValue;
            } else if (reader.Read()) {
                number = reader.Current.Class switch {
                    TokenClass.Number => (byte)(reader.CastCurrent<NumberToken>().Value & 0xFF),
                    TokenClass.Char => (byte)reader.CastCurrent<CharToken>().Value,
                    _ => throw ParserException.Create($"Expected Number or Char token, got {reader.Current.Class}", reader.Current),
                };
            } else {
                throw ParserException.Create($"Too few arguments. Expected Number or Char token", reader.Current);
            }

            instructionEncoder.Encode(instructionType, regDest, number, out instructionHigh, out instructionLow);
        }

        private void ParseRegisterInstruction(TokenReader reader, out BitArray instructionHigh, out BitArray instructionLow, McInstructionType instructionType, InstructionFormatAttribute instructionFormat) {
            Regs regDest = Regs.None;
            Regs regA = Regs.None;
            Regs regB = Regs.None;

            var regsToResolveCount = 3;

            if (instructionFormat.RegDestRestrictions != InstructionFormatAttribute.DefaultRegDestRestrictions) {
                regDest = instructionFormat.RegDestRestrictions;
                regsToResolveCount--;
            }

            if (instructionFormat.RegARestrictions != InstructionFormatAttribute.DefaultRegARestrictions) {
                regA = instructionFormat.RegARestrictions;
                regsToResolveCount--;
            }

            if (instructionFormat.RegBRestrictions != InstructionFormatAttribute.DefaultRegBRestrictions) {
                regB = instructionFormat.RegBRestrictions;
                regsToResolveCount--;
            }

            var originalRegsToResolveCount = regsToResolveCount;

            while (reader.Read() && regsToResolveCount-- > 0) {
                if (reader.Current is not RegisterToken registerToken) {
                    throw ParserException.Create($"Expected register token, got {reader.Current.Class}", reader.Current);
                }

                if (regDest == Regs.None) {
                    regDest = registerToken.Value;
                } else if (regA == Regs.None) {
                    regA = registerToken.Value;
                } else if (regB == Regs.None) {
                    regB = registerToken.Value;
                } else {
                    throw new Exception("This situation should not have happened");
                }
            }

            if (regDest == Regs.None || regA == Regs.None || regB == Regs.None) {
                throw ParserException.Create($"Too few register arguments. Expected {originalRegsToResolveCount} arguments, got {3 - regsToResolveCount} arguments", reader.Current);
            }

            instructionEncoder.Encode(instructionType, regDest, regA, regB, out instructionHigh, out instructionLow);
        }
    }
}

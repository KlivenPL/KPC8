using Assembler._Infrastructure;
using Assembler.Contexts;
using Assembler.Contexts.Labels;
using Assembler.Encoders;
using Assembler.Readers;
using Assembler.Tokens;
using KPC8._Infrastructure.Microcode.Attributes;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assembler.Parsers {
    class InstructionParser {
        private readonly InstructionEncoder instructionEncoder;
        private readonly InstructionsContext instructionsContext;
        private readonly LabelsContext labelsContext;

        public InstructionParser(InstructionsContext instructionsContext, InstructionEncoder instructionEncoder, LabelsContext labelsContext) {
            this.instructionsContext = instructionsContext;
            this.instructionEncoder = instructionEncoder;
            this.labelsContext = labelsContext;
        }

        public void Parse(TokenReader reader, out BitArray instructionHigh, out BitArray instructionLow) {
            instructionHigh = null;
            instructionLow = null;
            var identifier = reader.CastCurrent<IdentifierToken>();

            if (identifier.IsInstruction(out var instructionType)) {
                var instructionFormat = instructionsContext.GetInstructionFormat(instructionType);
                var originalReaderPos = reader.Position;

                var assReplacements = new List<ChangeToAssRegisterException>();

                while (true) {
                    try {
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
                        if (assReplacements.Count == 0) {
                            return;
                        } else {
                            var ex = assReplacements.Single();
                            throw new RegisterChangedToAssException(ex, instructionHigh, instructionLow);
                        }
                    } catch (InvalidTokenClassException ex) {
                        if (labelsContext.TryResolveInvalidTokenException(ex, out var resolvedToken)) {
                            reader.MoveTo(originalReaderPos);
                            reader.ReplaceToken(ex.RecievedToken, resolvedToken);
                        } else {
                            throw ex.ToParserException();
                        }
                    } catch (ChangeToAssRegisterException ex) {
                        reader.MoveTo(originalReaderPos);
                        reader.ReplaceToken(ex.TokenToChange, new RegisterToken(Regs.Ass, ex.TokenToChange.CodePosition, ex.TokenToChange.LineNumber, ex.TokenToChange.FilePath));
                        assReplacements.Add(ex);
                    }
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
                    try {
                        ValidateIsRegisterAllowed(instructionFormat.RegDestRestrictions, registerToken, instructionType);
                    } catch (ParserException) {
                        if (instructionFormat.RegDestRestrictions.HasFlag(Regs.Ass)) {
                            throw new ChangeToAssRegisterException(registerToken);
                        }
                    }
                    regDest = registerToken.Value;
                } else {
                    //  throw ParserException.Create($"Expected register, got {reader.Current.Class}", reader.Current);
                    throw new InvalidTokenClassException(reader.Current, TokenClass.Register);
                }
            }

            if (instructionFormat.ImmediateValue.HasValue) {
                number = (byte)instructionFormat.ImmediateValue;
            } else if (reader.Read()) {

                switch (reader.Current.Class) {
                    case TokenClass.Number:
                        var val = reader.CastCurrent<NumberToken>().Value;
                        if (val > byte.MaxValue && ((val & 0xFF00) >> 8 != 255)) {
                            throw ParserException.Create($"Value too big: {val}. Expected max value: {byte.MaxValue}", reader.Current);
                        }
                        number = (byte)val;
                        break;
                    case TokenClass.Char:
                        number = (byte)reader.CastCurrent<CharToken>().Value;
                        break;
                    default:
                        throw new InvalidTokenClassException(reader.Current, TokenClass.Number | TokenClass.Char);
                        //throw ParserException.Create($"Expected Number or Char token, got {reader.Current.Class}", reader.Current);
                }
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

            while (regsToResolveCount-- > 0 && reader.Read()) {
                if (reader.Current is not RegisterToken registerToken) {
                    throw new InvalidTokenClassException(reader.Current, TokenClass.Register);
                    //throw ParserException.Create($"Expected register token, got {reader.Current.Class}", reader.Current);
                }

                if (regDest == Regs.None) {
                    try {
                        ValidateIsRegisterAllowed(instructionFormat.RegDestRestrictions, registerToken, instructionType);
                    } catch (ParserException) {
                        if (instructionFormat.RegDestRestrictions.HasFlag(Regs.Ass)) {
                            throw new ChangeToAssRegisterException(registerToken);
                        }
                    }
                    regDest = registerToken.Value;
                } else if (regA == Regs.None) {
                    ValidateIsRegisterAllowed(instructionFormat.RegARestrictions, registerToken, instructionType);
                    regA = registerToken.Value;
                } else if (regB == Regs.None) {
                    ValidateIsRegisterAllowed(instructionFormat.RegBRestrictions, registerToken, instructionType);
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

        private void ValidateIsRegisterAllowed(Regs restrictions, RegisterToken registerToken, McInstructionType instructionType) {
            if (restrictions != Regs.None && !restrictions.HasFlag(registerToken.Value)) {
                throw ParserException.Create($"Invalid register in instruction {instructionType}, got {registerToken.Value}, expected {restrictions}", registerToken);
            }
        }
    }
}

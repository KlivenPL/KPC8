﻿using Assembler._Infrastructure;
using Assembler.Contexts.Labels;
using Assembler.Encoders;
using Assembler.Readers;
using Assembler.Tokens;
using KPC8._Infrastructure.Microcode.Attributes;
using KPC8.ProgRegs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assembler.Pseudoinstructions {
    abstract class PseudoinstructionBase {
        protected readonly LabelsContext labelsContext;

        public abstract PseudoinstructionType Type { get; }
        protected abstract IEnumerable<IEnumerable<BitArray>> ParseInner(TokenReader reader);
        protected InstructionEncoder InstructionEncoder { get; } = new InstructionEncoder();

        public BitArray[] Parse(TokenReader reader) {
            return ParseInner(reader).SelectMany(bitArray => bitArray).ToArray();
        }

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

        protected void SplitWord(ushort word, out byte lower, out byte higher) {
            lower = (byte)(word & 0x00FF);
            higher = (byte)((word >> 8) & 0x00FF);
        }

        protected bool DoesDestRegisterViolateDefaultRestrictions(RegisterToken registerToken) {
            var restrictions = InstructionFormatAttribute.DefaultRegDestRestrictions;

            if (restrictions != Regs.None && !restrictions.HasFlag(registerToken.Value)) {
                return true;
            }

            return false;
        }

        protected LabelNotResolvedException CreateLabelNotResolvedException(IdentifierToken identifierToken, ushort sizeInBytes, Func<ushort, BitArray[]> resolve) {
            return new LabelNotResolvedException(identifierToken, labelsContext.CurrentRegion, labelsContext.CurrentModule, sizeInBytes, resolve);
        }

        private T ParseNextParameter<T>(TokenReader reader) where T : IToken {
            if (reader.Read() && reader.Current is T parsed) {
                return parsed;
            } else {
                if (reader.Current != null) {
                    if (typeof(T) == typeof(NumberToken) && reader.Current is CharToken charToken) {
                        return (T)(object)new NumberToken((byte)charToken.Value, charToken.CodePosition, charToken.LineNumber, charToken.FilePath);
                    }

                    if (labelsContext.TryResolveInvalidToken<T>(reader.Current, out var resolvedToken)) {
                        return resolvedToken;
                    }
                }
                throw ParserException.Create($"Invalid {Type} pseudoinstruction parameter. Expected class {typeof(T).Name}, got {reader.Current.Class}", reader.Current);
            }
        }
    }
}

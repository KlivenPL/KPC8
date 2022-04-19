using Assembler._Infrastructure;
using Assembler.Builders;
using Assembler.Contexts;
using Assembler.Encoders;
using Assembler.Readers;
using Assembler.Tokens;
using System.Linq;

namespace Assembler.Commands {
    internal abstract class CommandBase {
        public abstract CommandType Type { get; }

        public void Parse(TokenReader reader, LabelsContext labelsContext, RomBuilder romBuilder) {
            ValidateRegions(reader, labelsContext);
            ParseInner(reader, labelsContext, romBuilder);
        }

        protected abstract string[] AcceptedRegions { get; }
        protected abstract void ParseInner(TokenReader reader, LabelsContext labelsContext, RomBuilder romBuilder);
        protected InstructionEncoder InstructionEncoder { get; } = new InstructionEncoder();

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

        protected virtual void ValidateRegions(TokenReader reader, LabelsContext labelsContext) {
            if (AcceptedRegions?.Any() != true) {
                throw new System.Exception("Commands must define accepted regions");
            }

            if (!AcceptedRegions.Any(x => x == labelsContext.CurrentReservedRegion)) {
                throw ParserException.Create($"Command {Type} can only be used in following reserved regions: {string.Join(", ", AcceptedRegions)}", reader.Current);
            }
        }

        private T ParseNextParameter<T>(TokenReader reader) where T : IToken {
            if (reader.Read() && reader.Current is T parsed) {
                return parsed;
            } else {
                throw ParserException.Create($"Invalid {Type} command parameter. Expected class {typeof(T).Name}, got {reader.Current.Class}", reader.Current);
            }
        }
    }
}

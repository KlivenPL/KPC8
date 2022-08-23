using Assembler.Builders;
using Assembler.Contexts;
using Assembler.DebugData;
using Assembler.Readers;
using Assembler.Tokens;

namespace Assembler.Commands {
    internal class DebugWrite : CommandBase {
        public override CommandType Type => CommandType.DebugWrite;

        protected override string[] AcceptedRegions => new[] { LabelsContext.CodeRegion };

        protected override void ParseInner(TokenReader reader, LabelsContext labelsContext, RomBuilder romBuilder) {
            ParseParameters<StringToken>(reader, out var dbgStrToken);
            AddDebugWriteSymbol(new DebugWriteSymbol((ushort)(romBuilder.NextAddress + 1), dbgStrToken.LineNumber, dbgStrToken.Value));
        }
    }
}

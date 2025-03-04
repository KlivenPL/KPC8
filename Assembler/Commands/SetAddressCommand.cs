using Assembler.Builders;
using Assembler.Contexts.Labels;
using Assembler.Readers;
using Assembler.Tokens;
using System.Collections.Generic;

namespace Assembler.Commands {
    internal class SetAddressCommand : CommandBase {
        public override CommandType Type => CommandType.SetAddress;

        public static IEnumerable<IToken> CreateCommandOnCompile(ushort address) {
            yield return new CommandToken(CommandType.SetAddress, -1, -1, null);
            yield return new NumberToken(address, -1, -1, null);
        }

        protected override CommandAllowedIn AcceptedRegions => CommandAllowedIn.ConstRegion | CommandAllowedIn.UserDefinedRegion;

        protected override void ParseInner(TokenReader reader, LabelsContext labelsContext, RomBuilder romBuilder) {
            ParseParameters<NumberToken>(reader, out var addressToJumpToken);
            romBuilder.NextAddress = addressToJumpToken.Value;
        }
    }
}

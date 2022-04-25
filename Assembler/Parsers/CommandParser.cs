using Assembler.Builders;
using Assembler.Contexts;
using Assembler.Readers;
using Assembler.Tokens;

namespace Assembler.Parsers {
    class CommandParser {
        private readonly CommandsContext commandsContext;
        private readonly LabelsContext labelsContext;

        public CommandParser(CommandsContext commandsContext, LabelsContext labelsContext) {
            this.commandsContext = commandsContext;
            this.labelsContext = labelsContext;
        }

        public void Parse(TokenReader reader, RomBuilder romBuilder) {
            var commandToken = reader.CastCurrent<CommandToken>();
            var command = commandsContext.GetCommand(commandToken.Value);
            command.Parse(reader, labelsContext, romBuilder);
        }
    }
}

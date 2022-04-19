using Assembler.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assembler.Contexts {
    class CommandsContext {
        private readonly Dictionary<CommandType, CommandBase> commands;

        public CommandsContext() {
            commands = typeof(CommandBase).Assembly.GetTypes()
                 .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(CommandBase)))
                 .Select(x => new { type = x, instance = Activator.CreateInstance(x) })
                 .ToDictionary(x => (CommandType)x.type.GetProperty("Type").GetValue(x.instance), x => (CommandBase)x.instance);
        }

        public CommandBase GetPseudoinstruction(CommandType commandType) {
            return commands[commandType];
        }
    }
}

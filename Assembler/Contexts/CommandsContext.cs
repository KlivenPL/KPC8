using Assembler.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Assembler.Contexts {
    class CommandsContext {
        private static readonly Dictionary<CommandType, CommandBase> commands;
        private static readonly Dictionary<CommandType, CommandBase> preParseCommands;

        static CommandsContext() {
            commands = typeof(CommandBase).Assembly.GetTypes()
                 .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(CommandBase)))
                 .Select(x => new { type = x, instance = Activator.CreateInstance(x) })
                 .ToDictionary(x => (CommandType)x.type.GetProperty("Type").GetValue(x.instance), x => (CommandBase)x.instance);

            preParseCommands = commands
                .Where(c => c.Value.GetType().GetMethod("PreParseInner", BindingFlags.Instance | BindingFlags.NonPublic).GetBaseDefinition().DeclaringType != c.Value.GetType().GetMethod("PreParseInner", BindingFlags.Instance | BindingFlags.NonPublic).DeclaringType)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public CommandBase GetCommand(CommandType commandType) {
            return commands[commandType];
        }

        public bool TryGetPreCommand(CommandType commandType, out CommandBase command) {
            return preParseCommands.TryGetValue(commandType, out command);
        }
    }
}

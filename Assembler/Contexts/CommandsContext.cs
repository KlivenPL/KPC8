#if DEBUG
using Assembler.Contexts.Signatures._Infrastructure;
using Assembler.Contexts.Signatures;
using Assembler.Tokens;
#endif
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

#if DEBUG
            SignaturesContext.AddSignatures(CreateCommandSignatures().ToList());
#endif
        }

        public CommandBase GetCommand(CommandType commandType) {
            return commands[commandType];
        }

        public bool TryGetPreCommand(CommandType commandType, out CommandBase command) {
            return preParseCommands.TryGetValue(commandType, out command);
        }

#if DEBUG

        private static IEnumerable<KpcSignature> CreateCommandSignatures() {
            return commands.Select(x => new KpcSignature {
                Name = Enum.GetName(x.Key),
                Type = KpcSignatureType.Command,
                Arguments = GetCommandArguments(x.Value).ToList(),
            });
        }

        private static IEnumerable<KpcArgument> GetCommandArguments(CommandBase command) {
            var instructions = MethodBodyReader.GetInstructions(command.GetType().GetMethod("ParseInner", BindingFlags.Instance | BindingFlags.NonPublic));

            foreach (Instruction instruction in instructions) {
                MethodInfo methodInfo = instruction.Operand as MethodInfo;

                if (methodInfo != null) {
                    if (methodInfo.IsGenericMethod) {
                        var genericArgs = methodInfo.GetGenericArguments();
                        foreach (var genericArg in genericArgs) {
                            yield return new KpcArgument { TokenClass = ResolveParameterType(genericArg) };
                        }

                        yield break;
                    }

                    TokenClass ResolveParameterType(Type parameterType) {
                        if (parameterType == typeof(IdentifierToken)) {
                            return TokenClass.Identifier;
                        }

                        if (parameterType == typeof(RegisterToken)) {
                            return TokenClass.Register;
                        }

                        if (parameterType == typeof(NumberToken)) {
                            return TokenClass.Number;
                        }

                        if (parameterType == typeof(StringToken)) {
                            return TokenClass.String;
                        }

                        if (parameterType == typeof(CharToken)) {
                            return TokenClass.Char;
                        }

                        throw new NotImplementedException();
                    }
                }
            }
        }
#endif
    }
}

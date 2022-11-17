#if DEBUG
using Assembler.Contexts.Signatures._Infrastructure;
using Assembler.Contexts.Signatures;
using Assembler.Tokens;
#endif
using Assembler.Contexts.Labels;
using Assembler.Pseudoinstructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Assembler.Contexts {
    class PseudoinstructionsContext {
        private readonly Dictionary<PseudoinstructionType, PseudoinstructionBase> pseudoinstructions;

        public PseudoinstructionsContext(LabelsContext labelsContext) {
            pseudoinstructions = typeof(PseudoinstructionBase).Assembly.GetTypes()
                 .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(PseudoinstructionBase)))
                 .Select(x => {
                     var instance = Activator.CreateInstance(x);
                     x.GetField("labelsContext", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(instance, labelsContext);
                     return new { type = x, instance };
                 })
                 .ToDictionary(x => (PseudoinstructionType)x.type.GetProperty("Type").GetValue(x.instance), x => (PseudoinstructionBase)x.instance);

#if DEBUG
            SignaturesContext.AddSignatures(CreatePseudoinstructionSignatures().ToList());
#endif
        }

        public PseudoinstructionBase GetPseudoinstruction(PseudoinstructionType instructionType) {
            return pseudoinstructions[instructionType];
        }

#if DEBUG

        private IEnumerable<KpcSignature> CreatePseudoinstructionSignatures() {
            return pseudoinstructions.Select(x => new KpcSignature {
                Name = Enum.GetName(x.Key),
                Type = KpcSignatureType.Pseudoinstruction,
                Arguments = GetPseudoinstructionArguments(x.Value).ToList(),
            });
        }

        private IEnumerable<KpcArgument> GetPseudoinstructionArguments(PseudoinstructionBase pseudoinstruction) {
            var instructions = MethodBodyReader.GetInstructions(pseudoinstruction.GetType().GetMethod("ParseInner", BindingFlags.Instance | BindingFlags.NonPublic));

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

#if DEBUG
using Assembler.Contexts.Signatures._Infrastructure;
using Assembler.Contexts.Signatures;
#endif
using KPC8._Infrastructure.Microcode.Attributes;
using KPC8.Microcode;
using KPC8.RomProgrammers.Microcode;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assembler.Contexts {
    class InstructionsContext {
        private readonly Dictionary<McInstructionType, InstructionFormatAttribute> instructionsFormats;

        public InstructionsContext() {
            instructionsFormats = new Dictionary<McInstructionType, InstructionFormatAttribute>();

            AddProceduralInstructionsFormats(typeof(NopInstruction));

            AddProceduralInstructionsFormats(typeof(InterruptProceduralInstructions));
            AddProceduralInstructionsFormats(typeof(JumpProceduralInstructions));
            AddProceduralInstructionsFormats(typeof(LoadProceduralInstructions));
            AddProceduralInstructionsFormats(typeof(LogicProceduralInstructions));
            AddProceduralInstructionsFormats(typeof(MathProceduralInstructions));
            AddProceduralInstructionsFormats(typeof(RegsProceduralInstructions));
            AddProceduralInstructionsFormats(typeof(StoreProceduralInstructions));

            AddConditionalInstructionsFormats(typeof(JumpConditionalInstructions));
#if DEBUG
            SignaturesContext.AddSignatures(CreateInstructionSignatures().ToList());
#endif
        }

        public InstructionFormatAttribute GetInstructionFormat(McInstructionType instructionType) {
            return instructionsFormats[instructionType];
        }

        private void AddProceduralInstructionsFormats(Type instructionsClassType) {
            var mis = instructionsClassType.GetMethods().Where(mi => mi.ReturnType.IsGenericType);

            foreach (var mi in mis) {
                var instrAttribute = mi.GetCustomAttributes(true).OfType<ProceduralInstructionAttribute>().Single();
                var instrFormatAttribute = mi.GetCustomAttributes(true).OfType<InstructionFormatAttribute>().Single();

                instructionsFormats.Add(instrAttribute.McInstructionType, instrFormatAttribute);
            }
        }

        private void AddConditionalInstructionsFormats(Type instructionsClassType) {
            var mis = instructionsClassType.GetMethods().Where(mi => mi.ReturnType.IsGenericType);

            foreach (var mi in mis) {
                var instrAttribute = mi.GetCustomAttributes(true).OfType<ConditionalInstructionAttribute>().Single();
                var instrFormatAttribute = mi.GetCustomAttributes(true).OfType<InstructionFormatAttribute>().Single();

                instructionsFormats.Add(instrAttribute.McInstructionType, instrFormatAttribute);
            }
        }

#if DEBUG

        private IEnumerable<KpcSignature> CreateInstructionSignatures() {
            return instructionsFormats.Select(x => new KpcSignature {
                Name = Enum.GetName(x.Key),
                Type = KpcSignatureType.Instruction,
                Arguments = ConvertFormatToArguments(x.Value).ToList(),
            });
        }

        private IEnumerable<KpcArgument> ConvertFormatToArguments(InstructionFormatAttribute format) {
            switch (format.InstructionFormat) {
                case McInstructionFormat.Register:
                    var regsToResolveCount = 3;

                    if (format.RegDestRestrictions != InstructionFormatAttribute.DefaultRegDestRestrictions) {
                        regsToResolveCount--;
                    }

                    if (format.RegARestrictions != InstructionFormatAttribute.DefaultRegARestrictions) {
                        regsToResolveCount--;
                    }

                    if (format.RegBRestrictions != InstructionFormatAttribute.DefaultRegBRestrictions) {
                        regsToResolveCount--;
                    }

                    for (int i = 0; i < regsToResolveCount; i++) {
                        yield return new KpcArgument { TokenClass = Tokens.TokenClass.Register };
                    }

                    break;
                case McInstructionFormat.Immediate:

                    if (format.RegDestRestrictions == InstructionFormatAttribute.DefaultRegDestRestrictions) {
                        yield return new KpcArgument { TokenClass = Tokens.TokenClass.Register };
                    }

                    if (!format.ImmediateValue.HasValue) {
                        yield return new KpcArgument { TokenClass = Tokens.TokenClass.Number };
                    }

                    break;
                default:
                    throw new NotImplementedException();
            }
        }

#endif
    }
}

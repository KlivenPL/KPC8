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
    }
}

using Assembler.Pseudoinstructions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assembler.Contexts {
    class PseudoinstructionsContext {
        private readonly Dictionary<PseudoinstructionType, PseudoinstructionBase> pseudoinstructions;

        public PseudoinstructionsContext() {
            pseudoinstructions = typeof(PseudoinstructionBase).Assembly.GetTypes()
                 .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(PseudoinstructionBase)))
                 .Select(x => new { type = x, instance = Activator.CreateInstance(x) })
                 .ToDictionary(x => (PseudoinstructionType)x.type.GetProperty("PseudoinstructionType").GetValue(x.instance), x => (PseudoinstructionBase)x.instance);
        }

        public PseudoinstructionBase GetPseudoinstruction(PseudoinstructionType instructionType) {
            return pseudoinstructions[instructionType];
        }
    }
}

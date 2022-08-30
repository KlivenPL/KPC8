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
        }

        public PseudoinstructionBase GetPseudoinstruction(PseudoinstructionType instructionType) {
            return pseudoinstructions[instructionType];
        }
    }
}

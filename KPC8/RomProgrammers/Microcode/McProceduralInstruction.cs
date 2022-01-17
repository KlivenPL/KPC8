using _Infrastructure.Enums;
using KPC8._Infrastructure.Microcode.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cs = KPC8.ControlSignals.ControlSignalType;

namespace KPC8.RomProgrammers.Microcode {
    public class McProceduralInstruction : McInstruction {
        private readonly int totalStepsCount;

        public McProceduralInstruction(string name, Cs[] steps, uint opcode) : base(name) {
            if (steps.Length == 0) {
                throw new System.Exception($"Procedural instruction {name} takes 0 steps");
            }

            totalStepsCount = steps.Length + PreInstructionSteps.Length;

            if (totalStepsCount > MaxTotalStepsCount) {
                throw new System.Exception($"Procedural instruction {name} takes too many total steps ({totalStepsCount}/{MaxTotalStepsCount})");
            }

            if (opcode > 0x37 || opcode < 0) {
                throw new System.Exception($"Procedural instruction {name}: Max opcode for procedural instruction is 0x37 ({opcode}/{0x37})");
            }

            InstructionSteps = steps;
            RomInstructionIndex = opcode;
        }

        public override int MaxTotalStepsCount => 16;
        public override uint RomInstructionIndex { get; }
        public override Cs[] PreInstructionSteps => FetchInstructions().ToArray();
        public override Cs OptionalPostInstructionStep => Cs.Ic_clr;
        public int PreAndInstructionStepsCount => PreInstructionSteps.Length + InstructionSteps.Length;
        public Cs[] InstructionSteps { get; }
        public override BitArray OpCode => ((McInstructionType)RomInstructionIndex).Get6BitsOPCode();

        public static McProceduralInstruction CreateFromSteps(Type classType, string stepsMethodName) {
            var mi = classType.GetMethod(stepsMethodName);
            var attribute = mi.GetCustomAttributes(true).OfType<ProceduralInstructionAttribute>().First();
            var devNameAttribute = attribute.McInstructionType.GetCustomAttribute<McInstructionNameAttribute>();
            var instruction = new McProceduralInstruction(devNameAttribute.DevName, ((IEnumerable<Cs>)mi.Invoke(null, null)).ToArray(), (uint)attribute.McInstructionType);
            return instruction;
        }

        private IEnumerable<Cs> FetchInstructions() {
            yield return Cs.Pc_oe | Cs.Mar_le_hi | Cs.Mar_le_lo;
            yield return Cs.Pc_ce | Cs.Rom_oe | Cs.Ir_le_hi;
            yield return Cs.Pc_ce | Cs.Mar_ce | Cs.Rom_oe | Cs.Ir_le_lo;
        }

        public override IEnumerable<Cs> BuildTotalSteps() {
            int currentLength = 0;
            foreach (var step in PreInstructionSteps) {
                currentLength++;
                yield return step;
            }

            for (int i = 0; i < InstructionSteps.Length; i++) {
                currentLength++;
                if (i == InstructionSteps.Length - 1) {
                    yield return InstructionSteps[i] | OptionalPostInstructionStep;
                } else {
                    yield return InstructionSteps[i];
                }
            }


            for (int i = currentLength; i < MaxTotalStepsCount; i++) {
                yield return OptionalPostInstructionStep;
            }
        }
    }
}

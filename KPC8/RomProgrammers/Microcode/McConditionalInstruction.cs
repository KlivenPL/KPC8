using _Infrastructure.Enums;
using KPC8._Infrastructure.Microcode.Attributes;
using KPC8.CpuFlags;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cs = KPC8.ControlSignals.ControlSignalType;

namespace KPC8.RomProgrammers.Microcode {
    public class McConditionalInstruction : McInstruction {
        private readonly Func<CpuFlag, IEnumerable<Cs>> stepsFunc;

        public McConditionalInstruction(string name, Func<CpuFlag, IEnumerable<Cs>> stepsFunc, uint opcode) : base(name) {
            if (opcode < 0x38 || opcode > 0x3F) {
                throw new System.Exception($"Conditional instruction {name}: Min opcode for procedural instruction is 0x38 ({opcode}/{0x38})");
            }

            this.stepsFunc = stepsFunc;
            var stepSets = BuildConditionalStepSets().ToArray();

            foreach (var steps in stepSets) {
                /*if (steps.Length == 0) {
                    throw new System.Exception($"Conditional instruction {name} takes 0 steps");
                }*/

                var totalStepsCount = steps.Length + PreInstructionSteps.Length;

                if (totalStepsCount > MaxTotalStepsCount) {
                    throw new System.Exception($"Conditional instruction {name} takes too many total steps ({totalStepsCount}/{MaxTotalStepsCount})");
                }
            }

            RomInstructionIndex = opcode;
        }

        public override int MaxTotalStepsCount => 8;
        public override uint RomInstructionIndex { get; }
        public override Cs[] PreInstructionSteps => FetchInstructions().ToArray();
        public override Cs OptionalPostInstructionStep => Cs.Ic_clr;
        public override BitArray OpCode => ((McInstructionType)RomInstructionIndex).Get6BitsOPCode();

        public static McConditionalInstruction CreateFromSteps(Type classType, string stepsMethodName) {
            var mi = classType.GetMethod(stepsMethodName);
            var attribute = mi.GetCustomAttributes(true).OfType<ConditionalInstructionAttribute>().First();
            var devNameAttribute = attribute.McInstructionType.GetCustomAttribute<McInstructionNameAttribute>();
            var instruction = new McConditionalInstruction(devNameAttribute.DevName, CreateStepsFunc(mi), (uint)attribute.McInstructionType);
            return instruction;

            static Func<CpuFlag, IEnumerable<Cs>> CreateStepsFunc(MethodInfo mi) {
                return (flags) => (IEnumerable<Cs>)mi.Invoke(null, new object[] { flags });
            }
        }

        private IEnumerable<Cs> FetchInstructions() {
            yield return Cs.Pc_oe | Cs.Mar_le_hi | Cs.Mar_le_lo;
            yield return Cs.Pc_ce | Cs.Rom_oe | Cs.Ir_le_hi;
            yield return Cs.Pc_ce | Cs.Mar_ce | Cs.Rom_oe | Cs.Ir_le_lo;
        }

        public IEnumerable<Cs[]> BuildConditionalStepSets() {
            for (int i = 0; i < 16; i++) {
                var flags = (CpuFlag)i;
                yield return stepsFunc(flags).ToArray();
            }
        }

        public IEnumerable<(CpuFlag flags, Cs[] steps)> BuildConditionalStepSetsWithCorrespondingFlags() {
            for (int i = 0; i < 16; i++) {
                var flags = (CpuFlag)i;
                yield return (flags, stepsFunc(flags).ToArray());
            }
        }

        public override IEnumerable<Cs> BuildTotalSteps() {
            foreach (var instructionSteps in BuildConditionalStepSets().ToArray()) {
                int currentLength = 0;
                foreach (var step in PreInstructionSteps) {
                    currentLength++;
                    yield return step;
                }

                for (int i = 0; i < instructionSteps.Length; i++) {
                    currentLength++;
                    if (i == instructionSteps.Length - 1) {

                        if (currentLength == MaxTotalStepsCount) {
                            yield return instructionSteps[i] | OptionalPostInstructionStep;
                        } else {
                            currentLength++;
                            yield return instructionSteps[i];
                            yield return OptionalPostInstructionStep;
                        }
                    } else {
                        yield return instructionSteps[i];
                    }
                }

                for (int i = currentLength; i < MaxTotalStepsCount; i++) {
                    yield return OptionalPostInstructionStep;
                }
            }
        }

        public IEnumerable<(CpuFlag flags, Cs step)> BuildTotalStepsWithCorrespondingFlags() {
            foreach (var (flags, instructionSteps) in BuildConditionalStepSetsWithCorrespondingFlags().ToArray()) {
                int currentLength = 0;
                foreach (var step in PreInstructionSteps) {
                    currentLength++;
                    yield return (flags, step);
                }

                for (int i = 0; i < instructionSteps.Length; i++) {
                    currentLength++;
                    if (i == instructionSteps.Length - 1) {
                        if (currentLength == MaxTotalStepsCount) {
                            yield return (flags, instructionSteps[i] | OptionalPostInstructionStep);
                        } else {
                            currentLength++;
                            yield return (flags, instructionSteps[i]);
                            yield return (flags, OptionalPostInstructionStep);
                        }
                    } else {
                        yield return (flags, instructionSteps[i]);
                    }
                }

                for (int i = currentLength; i < MaxTotalStepsCount; i++) {
                    yield return (flags, OptionalPostInstructionStep);
                }
            }
        }
    }
}

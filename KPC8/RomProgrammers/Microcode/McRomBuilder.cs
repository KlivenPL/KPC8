﻿using _Infrastructure.Enums;
using KPC8._Infrastructure.Microcode.Attributes;
using KPC8.ControlSignals;
using KPC8.CpuFlags;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KPC8.RomProgrammers.Microcode {
    public class McRomBuilder {

        private readonly McInstruction[] instructions;
        private McInstruction defaultInstruction;

        public McRomBuilder(int instructionSetLength) {
            instructions = new McInstruction[instructionSetLength];
        }

        public McInstruction[] GetInstructions => instructions;

        public McRomBuilder SetDefaultInstruction(McInstruction defaultInstruction) {
            if (this.defaultInstruction != null) {
                throw new System.Exception($"Default instruction is already set to: {defaultInstruction.Name}. Collision with new instruction: {defaultInstruction.Name}");
            }
            this.defaultInstruction = defaultInstruction;

            return this;
        }

        public McRomBuilder AddInstructions(IEnumerable<McInstruction> newInstructions) {
            foreach (var instruction in newInstructions) {
                AddInstruction(instruction);
            }
            return this;
        }

        public McRomBuilder AddInstruction(McInstruction newInstruction, int indexOffset = 0) {
            var instrIndex = newInstruction.RomInstructionIndex + indexOffset;
            if (instructions[instrIndex] != null) {
                var ins = instructions[instrIndex];
                throw new System.Exception($"An instruction {ins.Name} already exists on address: {ins.RomInstructionIndex}. Collision with new instruction: {newInstruction.Name}");
            }

            instructions[instrIndex] = newInstruction;
            return this;
        }

        public McRomBuilder FindAndAddAllProceduralInstructions() {
            IEnumerable<(IEnumerable<ControlSignalType> steps, ProceduralInstructionAttribute attribute)> stepsWithAttributes = typeof(ProceduralInstructionAttribute).Assembly.GetTypes()
                      .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public))
                      .Where(m => m.GetCustomAttributes(typeof(ProceduralInstructionAttribute), false).Length > 0)
                      .Select(m => ((IEnumerable<ControlSignalType>)m.Invoke(null, null), (ProceduralInstructionAttribute)m.GetCustomAttribute(typeof(ProceduralInstructionAttribute), false))).ToList();

            foreach (var (steps, attribute) in stepsWithAttributes) {
                var devNameAttribute = attribute.McInstructionType.GetCustomAttribute<McInstructionNameAttribute>();
                var instruction = new McProceduralInstruction(devNameAttribute.DevName, steps.ToArray(), (uint)attribute.McInstructionType);
                AddInstruction(instruction);
            }

            return this;
        }

        public McRomBuilder FindAndAddAllConditionalInstructions() {
            IEnumerable<(Func<CpuFlag, IEnumerable<ControlSignalType>> stepsFunc, ConditionalInstructionAttribute attribute)> stepsWithAttributes = typeof(ConditionalInstructionAttribute).Assembly.GetTypes()
                      .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public))
                      .Where(m => m.GetCustomAttributes(typeof(ConditionalInstructionAttribute), false).Length > 0)
                      .Select(m => (CreateStepsFunc(m), (ConditionalInstructionAttribute)m.GetCustomAttribute(typeof(ConditionalInstructionAttribute), false))).ToList();

            foreach (var (stepsFunc, attribute) in stepsWithAttributes) {
                var devNameAttribute = attribute.McInstructionType.GetCustomAttribute<McInstructionNameAttribute>();
                var instruction = new McConditionalInstruction(devNameAttribute.DevName, stepsFunc, (uint)attribute.McInstructionType);
                AddInstruction(instruction, 8);
            }

            return this;

            static Func<CpuFlag, IEnumerable<ControlSignalType>> CreateStepsFunc(MethodInfo mi) {
                return (flags) => (IEnumerable<ControlSignalType>)mi.Invoke(null, new object[] { flags });
            }
        }

        public BitArray[] Build() {
            if (defaultInstruction == null) {
                throw new System.Exception("Default microcode instruction is not set.");
            }

            return BuildInternal().ToArray();
        }

        private IEnumerable<BitArray> BuildInternal() {
            for (int i = 0; i < instructions.Length; i++) {
                if (instructions[i] == null)
                    instructions[i] = defaultInstruction;

                foreach (var step in instructions[i].BuildTotalSteps()) {
                    yield return step.ToBitArray();
                }
            }
        }
    }
}

using Infrastructure.BitArrays;
using KPC8.ControlSignals;
using KPC8.ExternalModules;
using KPC8.Microcode;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using Simulation.Loops;
using System;
using System.Collections;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace Tests.KPC8Tests.Integration.ExternalModules {
    public class KTimerExternalModuleTests : ExternalModuleTestBase {
        public KTimerExternalModuleTests(ITestOutputHelper debug) : base(debug) {

        }

        [Theory]
        [InlineData(10)]
        [InlineData(30)]
        [InlineData(239)]
        public void KTimerConfigureFrequency(byte frequency) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(StoreProceduralInstructions), nameof(StoreProceduralInstructions.Sbext));
            var kTimerAddress = BitArrayHelper.FromUShortLE(0x2137);
            var configurationValue = BitArrayHelper.FromUShortLE(frequency);

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh, out var instructionLow);

            var romData = new BitArray[] { instructionHigh, instructionLow };

            KTimerExternalModule kTimerExternalModule = null;
            Func<SimulationLoop> getKTimerModuleSimLoop = null;

            var cp = BuildPcModules(builder => ConnectKTimer(builder, kTimerAddress.ToUShortLE(), out kTimerExternalModule, out getKTimerModuleSimLoop), romData, out var modules);
            var simLoop = getKTimerModuleSimLoop();
            using var kTimerSimRunner = SimulationLoopRunner.RunInNewThread(simLoop);

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), configurationValue);
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), kTimerAddress);
            Thread.Sleep(100);

            StepThroughProceduralInstruction(modules, instruction);
            Thread.Sleep(100);

            Assert.Equal(frequency, kTimerExternalModule.GetFrequency());
        }

        [Theory]
        [InlineData(0xF0)]
        [InlineData(0xF6)]
        [InlineData(0xFF)]
        public void KTimerConfigureInterruptCode(byte irrCode) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(StoreProceduralInstructions), nameof(StoreProceduralInstructions.Sbext));
            var kTimerAddress = BitArrayHelper.FromUShortLE(0x2137);
            var configurationValue = BitArrayHelper.FromUShortLE(irrCode);

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh, out var instructionLow);

            var romData = new BitArray[] { instructionHigh, instructionLow };

            KTimerExternalModule kTimerExternalModule = null;
            Func<SimulationLoop> getKTimerModuleSimLoop = null;

            var cp = BuildPcModules(builder => ConnectKTimer(builder, kTimerAddress.ToUShortLE(), out kTimerExternalModule, out getKTimerModuleSimLoop), romData, out var modules);
            var simLoop = getKTimerModuleSimLoop();
            using var kTimerSimRunner = SimulationLoopRunner.RunInNewThread(simLoop);

            modules.Registers.SetWholeRegContent(Regs.T1.GetIndex(), configurationValue);
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), kTimerAddress);
            Thread.Sleep(100);

            StepThroughProceduralInstruction(modules, instruction);
            Thread.Sleep(100);

            Assert.Equal(0, kTimerExternalModule.GetFrequency());
            Assert.Equal(irrCode & 0x0F, kTimerExternalModule.GetIrrCode());
        }

        private CpuBuilder ConnectKTimer(CpuBuilder builder, ushort address, out KTimerExternalModule kTimerModule, out Func<SimulationLoop> getKTimerModuleSimLoop) {
            return builder.AddKTimer("KTimer", address, out kTimerModule, out getKTimerModuleSimLoop);
        }
    }
}

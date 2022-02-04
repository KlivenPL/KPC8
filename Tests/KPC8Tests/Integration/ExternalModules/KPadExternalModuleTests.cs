using ExternalDevices.HID;
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
using Tests._Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Tests.KPC8Tests.Integration.ExternalModules {
    public class KPadExternalModuleTests : ExternalModuleTestBase {
        public KPadExternalModuleTests(ITestOutputHelper debug) : base(debug) {

        }

        [Theory]
        [InlineData(0x01)]
        [InlineData(0x02)]
        [InlineData(0x911)]
        [InlineData(0x2137)]
        public void KPadLoadButtons(ushort kPadAddressStr1) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(LoadProceduralInstructions), nameof(LoadProceduralInstructions.Lbext));

            var kPadAddress = BitArrayHelper.FromUShortLE(kPadAddressStr1);
            var simulatedButtons = KPadButtons.A | KPadButtons.Right;
            var simulatedButtonsVal = BitArrayHelper.FromByteLE((byte)simulatedButtons);

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh, out var instructionLow);

            var romData = new BitArray[] { instructionHigh, instructionLow };

            KPadExternalModule kPadExternalModule = null;
            Func<SimulationLoop> getKPadModuleSimLoop = null;
            var cp = BuildPcModules(builder => ConnectKPad(builder, kPadAddressStr1, out kPadExternalModule, out getKPadModuleSimLoop), romData, out var modules);
            var simLoop = getKPadModuleSimLoop();
            using var kPadSimRunner = SimulationLoopRunner.RunInNewThread(simLoop);

            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), kPadAddress);
            kPadExternalModule.SimulateButtons(simulatedButtons);

            Thread.Sleep(100);

            StepThroughProceduralInstruction(modules, instruction);
            BitAssert.Equality(simulatedButtonsVal, modules.Registers.GetLoRegContent(Regs.T1.GetIndex()));
        }

        [Theory]
        [InlineData(0x01, 0x02)]
        [InlineData(0x02, 0x03)]
        [InlineData(0x911, 0x912)]
        [InlineData(0x2137, 0x2138)]
        public void TwoKPadsLoadButtons(ushort kPadAddressStr1, ushort kPadAddressStr2) {
            var instruction = McProceduralInstruction.CreateFromSteps(typeof(LoadProceduralInstructions), nameof(LoadProceduralInstructions.Lbext));

            var kPadAddress1 = BitArrayHelper.FromUShortLE(kPadAddressStr1);
            var simulatedButtons1 = KPadButtons.A | KPadButtons.Right;
            var simulatedButtonsVal1 = BitArrayHelper.FromByteLE((byte)simulatedButtons1);

            var kPadAddress2 = BitArrayHelper.FromUShortLE(kPadAddressStr2);
            var simulatedButtons2 = KPadButtons.B | KPadButtons.Left;
            var simulatedButtonsVal2 = BitArrayHelper.FromByteLE((byte)simulatedButtons2);

            EncodeInstruction(instruction, Regs.Zero, Regs.T1, Regs.T2, out var instructionHigh1, out var instructionLow1);
            EncodeInstruction(instruction, Regs.Zero, Regs.T2, Regs.T4, out var instructionHigh2, out var instructionLow2);

            var romData = new BitArray[] { instructionHigh1, instructionLow1, instructionHigh2, instructionLow2 };

            KPadExternalModule kPadExternalModule1 = null;
            Func<SimulationLoop> getKPadModuleSimLoop1 = null;
            KPadExternalModule kPadExternalModule2 = null;
            Func<SimulationLoop> getKPadModuleSimLoop2 = null;
            var cp = BuildPcModules(builder => ConnectTwoKPads(builder, kPadAddressStr1, kPadAddressStr2, out kPadExternalModule1, out getKPadModuleSimLoop1, out kPadExternalModule2, out getKPadModuleSimLoop2), romData, out var modules);
            var simLoop1 = getKPadModuleSimLoop1();
            var simLoop2 = getKPadModuleSimLoop2();
            using var kPadSimRunner1 = SimulationLoopRunner.RunInNewThread(simLoop1);
            using var kPadSimRunner2 = SimulationLoopRunner.RunInNewThread(simLoop2);

            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), kPadAddress1);
            modules.Registers.SetWholeRegContent(Regs.T4.GetIndex(), kPadAddress2);
            kPadExternalModule1.SimulateButtons(simulatedButtons1);
            kPadExternalModule2.SimulateButtons(simulatedButtons2);

            Thread.Sleep(100);

            StepThroughProceduralInstruction(modules, instruction);
            StepThroughProceduralInstruction(modules, instruction);

            BitAssert.Equality(simulatedButtonsVal1, modules.Registers.GetLoRegContent(Regs.T1.GetIndex()));
            BitAssert.Equality(simulatedButtonsVal2, modules.Registers.GetLoRegContent(Regs.T2.GetIndex()));
        }

        private CpuBuilder ConnectKPad(CpuBuilder builder, ushort address, out KPadExternalModule kPadModule, out Func<SimulationLoop> getKPadModuleSimLoop) {
            return builder.AddKPad("KPad", address, out kPadModule, out getKPadModuleSimLoop);
        }

        private CpuBuilder ConnectTwoKPads(CpuBuilder builder, ushort address1, ushort address2, out KPadExternalModule kPadModule1, out Func<SimulationLoop> getKPadModuleSimLoop1, out KPadExternalModule kPadModule2, out Func<SimulationLoop> getKPadModuleSimLoop2) {
            return builder
                .AddKPad("KPad1", address1, out kPadModule1, out getKPadModuleSimLoop1)
                .AddKPad("KPad2", address2, out kPadModule2, out getKPadModuleSimLoop2);
        }
    }
}

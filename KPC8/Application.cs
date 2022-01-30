using Components.Clocks;
using Components.Signals;
using ExternalDevices.HID;
using Infrastructure.BitArrays;
using KPC8.Clocks;
using KPC8.ControlSignals;
using KPC8.ExternalModules;
using KPC8.ProgRegs;
using Simulation.Loops;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KPC8 {
    class Application {

        private Clock mainClock;
        private SimulationLoop mainLoop;

        private CsPanel cp;
        private ModulePanel modules;
        private KPad kpad;

        public Application() {
            Create();
        }

        public void Run() {
            var originalStr = "00001000 01011001";
            var original = BitArrayHelper.FromString(originalStr);
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), original);

            for (int i = 0; i < 100000000; i++) {
                mainLoop.Loop();
            }
        }


        private List<Signal> cycleSignals = new List<Signal>();
        private void Enable(Signal signal) {
            signal.Value = true;
            cycleSignals.Add(signal);
        }

        private void Create(BitArray[] initialMemory = null) {
            var instrHi = BitArrayHelper.FromString("01110000");
            var instrlo = BitArrayHelper.FromString("01010000");
            initialMemory = Enumerable.Range(0, ushort.MaxValue / 2).Select(i => i % 2 == 0 ? instrHi : instrlo).ToArray();

            var clk = Signal.Factory.Create("MainClock");
            var clkBar = Signal.Factory.Create("MainClockBar");
            var clkParameters = ClockType.MainClock.GetClockParameters();

            using (var loopBuilder = SimulationLoopBuilder.CreateAsCurrent().SetName("MainSimulation")) {

                mainClock = new Clock(clk, clkBar, clkParameters.ClockMode, clkParameters.PeriodInTicks);
                cp = new CpuBuilder(mainClock)
                   .WithControlModule(null, true)
                   .WithMemoryModule(initialMemory, null)
                   .WithRegistersModule()
                   .WithAluModule()
                   .AddKPad("KPad1", 0x00, out var kPadModule, out var getKPadSimLoop)
                   .BuildWithModulesAccess(out modules);

                mainLoop = loopBuilder.Build();
                SimulationLoopRunner.RunInNewThread(getKPadSimLoop(), out var kPad1Thread);
            }
        }

        private static bool ShouldEscape() {
            var key = Console.ReadKey();

            switch (key.Key) {
                case ConsoleKey.Enter:
                    break;
                case ConsoleKey.Escape:
                    return true;
            }

            return false;
        }
    }
}

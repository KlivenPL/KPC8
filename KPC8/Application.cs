using Autofac.Features.AttributeFilters;
using Components.Clocks;
using Components.Signals;
using KPC8.Clocks;
using KPC8.ControlSignals;
using Simulation.Loops;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KPC8 {
    class Application {

        private readonly Clock mainClock;
        private readonly SimulationLoop loop;

        private CsPanel cp;

        public Application(
            [KeyFilter(ClockType.MainClock)] Clock mainClock,
            SimulationLoop loop) {

            this.mainClock = mainClock;
            this.loop = loop;

            Create();
        }

        public void Run() {
            /*while (true) {

                MakeTickAndWait();
                if (ShouldEscape()) break;
            }*/
            while (true) {
                loop.Loop();
            }
        }


        private List<Signal> cycleSignals = new List<Signal>();
        private void Enable(Signal signal) {
            signal.Value = true;
            cycleSignals.Add(signal);
        }

        private void Create(BitArray[] initialMemory = null) {
            cp = new CpuBuilder(mainClock)
                .WithControlModule(null, true)
                .WithMemoryModule(null, null)
                .WithRegistersModule()
                .WithAluModule()
                .Build();

        }

        private void MakeTickAndWait() {
            loop.Loop();
            mainClock.MakeTick();
            while (mainClock.IsManualTickInProgress) {
                loop.Loop();
            }

            foreach (var sig in cycleSignals) {
                sig.Value = false;
            }

            cycleSignals.Clear();

            /*Console.WriteLine($"#{cycleNumber++}");
            Console.WriteLine($"PC:\t{pc.Content.ToBitStringWithDecAndHexLE()}");
            Console.WriteLine($"Bus:\t{dataBus.PeakAll().ToBitStringWithDecAndHexLE()}");
            Console.WriteLine($"MAR:\t{mar.Content.ToBitStringWithDecAndHexLE()} -> RAM: \t{ram.Content[mar.Content.ToIntLE()].ToBitStringWithDecAndHexLE()}");
            Console.WriteLine($"A:\t{regA.Content.ToBitStringWithDecAndHexLE()}");
            Console.WriteLine($"B:\t{regB.Content.ToBitStringWithDecAndHexLE()}");
            Console.WriteLine($"ALU:\t{aluAdder.Content.ToBitStringWithDecAndHexLE()}");
            Console.WriteLine();*/
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

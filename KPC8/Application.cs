using Autofac.Features.AttributeFilters;
using Components.Clocks;
using Components.Signals;
using Infrastructure.BitArrays;
using KPC8.Clocks;
using KPC8.ControlSignals;
using KPC8.ProgRegs;
using Simulation.Loops;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KPC8 {
    class Application {

        private readonly Clock mainClock;
        private readonly SimulationLoop loop;

        private CsPanel cp;
        private ModulePanel modules;

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
            var originalStr = "00001000 01011001";
            var original = BitArrayHelper.FromString(originalStr);
            modules.Registers.SetWholeRegContent(Regs.T2.GetIndex(), original);
           // while (true) {

                for (int i = 0; i < 100000000; i++) {
                    loop.Loop();
                    /*mainClock.MakeTick();
                    loop.Loop();*/
                //Console.WriteLine(mainClock.Cycles +  " " + mainClock.Clk + " " + modules.Registers.GetWholeRegContent(Regs.T1.GetIndex()).ToPrettyBitString());
                }
            //}
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

            cp = new CpuBuilder(mainClock)
                .WithControlModule(null, true)
               .WithMemoryModule(initialMemory, null)
               .WithRegistersModule()
               .WithAluModule()
               .BuildWithModulesAccess(out modules);

            /*loop.Loop();
            loop.Loop();
            loop.Loop();
            loop.Loop();*/
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

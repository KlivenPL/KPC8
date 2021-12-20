using Autofac.Features.AttributeFilters;
using Components.Clocks;
using KPC8.Clocks;
using Simulation.Frames;
using Simulation.Loops;
using System.Diagnostics;

namespace KPC8 {
    class Application {

        private readonly Clock mainClock;
        private readonly SimulationLoop loop;

        public Application(
            [KeyFilter(ClockType.MainClock)] Clock mainClock,
            SimulationLoop loop) {

            this.mainClock = mainClock;
            this.loop = loop;
        }

        public void Run() {
            int t = 0;
            int f = 0;

            while (FrameInfo.Ticks < (ulong)Stopwatch.Frequency * 10L) {
                loop.Loop();
                if (mainClock.Clk) {
                    t++;
                } else {
                    f++;
                }

            }
            System.Console.WriteLine($"t: {t} f: {f}");
        }
    }
}

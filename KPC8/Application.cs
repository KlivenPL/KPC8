using Autofac.Features.AttributeFilters;
using Components.Clocks;
using KPC8.Clocks;
using Simulation.Frames;
using Simulation.Loops;

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

            while (FrameInfo.Time < 10f) {
                loop.Loop();
                System.Console.WriteLine(mainClock.Clk);
            }
        }
    }
}

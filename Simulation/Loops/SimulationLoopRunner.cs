using System.Threading;

namespace Simulation.Loops {
    public static class SimulationLoopRunner {
        public static void RunInNewThread(SimulationLoop simLoop, out Thread thread) {
            thread = new Thread(() => {
                System.Timers.Timer timer = new System.Timers.Timer(10);
                timer.Elapsed += (x, d) => simLoop.Loop();
                timer.AutoReset = true;
                timer.Enabled = true;
            });

            thread.Start();
        }
    }
}

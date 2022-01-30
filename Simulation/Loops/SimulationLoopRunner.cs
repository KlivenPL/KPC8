using System;
using System.Threading;

namespace Simulation.Loops {
    public class SimulationLoopRunner : IDisposable {
        private readonly Thread thread;
        private readonly CancellationTokenSource cancellationTokenSource;

        private SimulationLoopRunner(Thread thread, CancellationTokenSource cancellationTokenSource) {
            this.thread = thread;
            this.cancellationTokenSource = cancellationTokenSource;
            thread.Start();
        }

        public static SimulationLoopRunner RunInNewThread(SimulationLoop simLoop) {
            var cancellationTokenSource = new CancellationTokenSource();
            var thread = new Thread(() => SetUpLoop(simLoop, cancellationTokenSource.Token));
            var runner = new SimulationLoopRunner(thread, cancellationTokenSource);
            return runner;
        }

        public void Kill() {
            cancellationTokenSource.Cancel();
        }

        private static void SetUpLoop(SimulationLoop simLoop, CancellationToken token) {
            System.Timers.Timer timer = new System.Timers.Timer(10);
            timer.Elapsed += (x, d) => {
                token.ThrowIfCancellationRequested();
                simLoop.Loop();
            };
            timer.AutoReset = true;
            timer.Enabled = true;
            simLoop.Loop();
            simLoop.Loop();
            simLoop.Loop();
        }

        public void Dispose() {
            Kill();
        }
    }
}

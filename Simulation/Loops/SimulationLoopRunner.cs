using System;
using System.Threading;

namespace Simulation.Loops {
    public class SimulationLoopRunner : IDisposable {
        private readonly Thread thread;
        private readonly CancellationTokenSource cancellationTokenSource;
        private static object lockObject = new object();

        private SimulationLoopRunner(Thread thread, CancellationTokenSource cancellationTokenSource, ManualResetEventSlim waitForThreadStartHandle) {
            this.thread = thread;
            this.cancellationTokenSource = cancellationTokenSource;
            thread.Start();
            waitForThreadStartHandle.Wait();
        }

        public static SimulationLoopRunner RunInNewThread(SimulationLoop simLoop) {
            var cancellationTokenSource = new CancellationTokenSource();
            var waitForThreadStartHandle = new ManualResetEventSlim(false);
            var thread = new Thread(() => SetUpLoop(simLoop, cancellationTokenSource.Token, waitForThreadStartHandle)) { Name = $"{simLoop.LoopName}Runner" };
            var runner = new SimulationLoopRunner(thread, cancellationTokenSource, waitForThreadStartHandle);
            return runner;
        }

        public void Kill() {
            cancellationTokenSource.Cancel();
        }

        private static void SetUpLoop(SimulationLoop simLoop, CancellationToken token, ManualResetEventSlim waitForThreadStartHandle) {
            simLoop.Loop();
            simLoop.Loop();
            simLoop.Loop();

            waitForThreadStartHandle.Set();

            while (!token.IsCancellationRequested) {
                lock (lockObject) {
                    simLoop.Loop();
                }

                simLoop.Loop();
                simLoop.Loop();
                simLoop.Loop();
            }

            simLoop.Dispose();
        }

        public void Dispose() {
            Kill();
        }
    }
}

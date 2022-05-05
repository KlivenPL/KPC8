using Runner.Build;
using Simulation.Loops;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Runner.Player {
    public class PlaySession {
        private readonly ManualResetEventSlim runEvent;
        // private readonly object syncObject;
        private readonly KPC8Build kpc;

        private List<SimulationLoopRunner> externalSlRunners;
        private bool terminate = false;

        #region DebugSession Events

        internal event Action PausedEvent;

        #endregion

        internal PlaySession(KPC8Build kpc, ManualResetEventSlim runEvent, object syncObject) {
            this.runEvent = runEvent;
            this.kpc = kpc;
            //   this.syncObject = syncObject;

            externalSlRunners = new List<SimulationLoopRunner>();
        }

        internal void Start(CancellationToken cancellationToken) {
            foreach (var externalModuleSl in kpc.ExternalSimulationLoops) {
                externalSlRunners.Add(SimulationLoopRunner.RunInNewThread(externalModuleSl));
            }

            PlayerLoop(cancellationToken);
        }

        private void PlayerLoop(CancellationToken cancellationToken) {
            do {
                // lock(syncObject)
                if (!runEvent.IsSet) {
                    HandlePause();
                }

                try {
                    runEvent.Wait(cancellationToken);
                } catch (OperationCanceledException) {
                    terminate = true;
                    continue;
                }

                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();

                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();

                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();

                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();

                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();

                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();

                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();

                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();

                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();

                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();

                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();

                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();

                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();

                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();

                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();

                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();

            } while (!terminate);

            foreach (var externalSlRunner in externalSlRunners) {
                externalSlRunner.Kill();
            }

            externalSlRunners.Clear();
            externalSlRunners = null;
        }

        private void HandlePause() {
            PausedEvent();
        }

        internal void Continue() {
            runEvent.Set();
        }

        internal void RequestPause() {
            runEvent.Reset();
        }

        internal void RequestTerminate() {
            //   lock (syncObject) {
            terminate = true;
            //   }
        }
    }
}

using Abstract;
using System;
using System.Threading;

namespace Runner.Player {
    public class PlaySession {
        private readonly ManualResetEventSlim runEvent;
        private readonly IKpcBuild kpc;
        private readonly IEmulationController emulationController;
        private bool terminate = false;

        #region DebugSession Events

        internal event Action PausedEvent;

        #endregion

        internal PlaySession(IKpcBuild kpc, IEmulationController emulationController, ManualResetEventSlim runEvent, object syncObject) {
            this.runEvent = runEvent;
            this.kpc = kpc;
            this.emulationController = emulationController;
        }

        internal void Start(CancellationToken cancellationToken) {
            emulationController.InitializePlay();

            Thread.Sleep(100);

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

                emulationController.Execute();

            } while (!terminate);

            emulationController.Terminate();
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
            terminate = true;
        }
    }
}

using Runner._Infrastructure;
using Runner.Build;
using Runner.Configuration;
using System;
using System.Threading;

namespace Runner.Player {
    public class PlaySessionController : IKPC8SessionController {
        private readonly KPC8Build kpc;

        private readonly object syncObject;
        private readonly ManualResetEventSlim runEvent;

        private readonly PlaySession playSession;
        private readonly CancellationTokenSource cts;
        private Thread playThread;

        #region DebugSessionController Events

        public event Action<int> ExitedEvent;
        public event Action TerminatedEvent;
        public event Action PausedEvent;

        #endregion

        public bool IsStarted => playThread?.IsAlive == true;

        KPC8Build IKPC8SessionController.GetKPC8Build => kpc;

        private PlaySessionController(KPC8Build kpc) {
            this.kpc = kpc;
            syncObject = new object();
            runEvent = new ManualResetEventSlim(true);
            playSession = new PlaySession(kpc, runEvent, syncObject);
            cts = new CancellationTokenSource();
        }

        public void StartPlaying() {
            SubscribeToPassedEvents();
            playThread = new Thread(() => playSession.Start(cts.Token));
            playThread.Name = "Main KPC8 play thread";
            playThread.Priority = ThreadPriority.Highest;
            playThread.Start();
        }

        private void SubscribeToPassedEvents() {
            playSession.PausedEvent += PausedEvent;
        }

        public void Continue() {
            playSession.Continue();
        }

        public void Pause() {
            playSession.RequestPause();
        }

        public void Terminate() {
            cts.Cancel();
            playSession.RequestTerminate();
            ExitedEvent(0);

            playThread.Join(5000);
            TerminatedEvent();
        }

        public class Factory {
            public static PlaySessionController Create(KPC8Configuration kpcConfig) {
                var kpcBuild = new KPC8Builder(kpcConfig).Build();
                return new PlaySessionController(kpcBuild);
            }
        }
    }
}

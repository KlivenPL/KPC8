using Abstract;
using LightweightEmulator.Configuration;
using LightweightEmulator.Pipelines;
using Runner._Infrastructure;
using Runner.Build;
using Runner.Configuration;
using Runner.EmulationControl;
using System;
using System.Threading;

namespace Runner.Player {
    public class PlaySessionController : IKPC8SessionController {
        private readonly IKpcBuild kpc;
        private readonly IEmulationController emulationController;

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

        IKpcBuild IKPC8SessionController.GetKPC8Build => kpc;

        private PlaySessionController(IKpcBuild kpc, IEmulationController emulationController) {
            this.kpc = kpc;
            this.emulationController = emulationController;
            syncObject = new object();
            runEvent = new ManualResetEventSlim(true);
            playSession = new PlaySession(kpc, emulationController, runEvent, syncObject);
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
                var emulationController = new KPC8EmulationController(kpcBuild);
                return new PlaySessionController(kpcBuild, emulationController);
            }

            public static PlaySessionController CreateLw(LwKpcConfiguration kpcConfig) {
                var kpcBuild = new LwKpcBuilder(kpcConfig).Build();
                var emulationController = new LwEmulationController(kpcBuild);
                return new PlaySessionController(kpcBuild, emulationController);
            }
        }
    }
}

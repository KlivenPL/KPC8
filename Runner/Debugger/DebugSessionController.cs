using Runner.Build;
using Runner.Configuration;
using Runner.Debugger.DebugData;
using Runner.Debugger.Enums;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Runner.Debugger {
    public class DebugSessionController {
        private readonly object syncObject;
        private readonly ManualResetEventSlim runEvent;

        private readonly DebugSession debugSession;
        private Thread debugThread;

        #region DebugSessionController Events

        public event Action<int> ExitedEvent;
        public event Action TerminatedEvent;

        #endregion

        #region DebugSession passed Events

        public event Action InitializedEvent;
        public event Action<DebugInfo> InvalidatedEvent;
        public event Action<OutputType, string> OutputEvent;
        public event Action<PauseReasonType, DebugInfo> PausedEvent;

        #endregion

        public bool IsStarted => debugThread.IsAlive;

        private DebugSessionController(DebugSessionConfiguration configuration, KPC8Build kpc) {
            syncObject = new object();
            runEvent = new ManualResetEventSlim();
            debugSession = new DebugSession(configuration, kpc, runEvent, syncObject);

            SubscribeToPassedEvents();
        }

        public void StartDebugging() {
            debugThread = new Thread(debugSession.Start);
            debugThread.Name = "Main debug thread";
            debugThread.Start();
        }

        private void SubscribeToPassedEvents() {
            debugSession.InitializedEvent += InitializedEvent;
            debugSession.InvalidatedEvent += InvalidatedEvent;
            debugSession.OutputEvent += OutputEvent;
            debugSession.PausedEvent += PausedEvent;
        }

        public IEnumerable<BreakpointInfo> GetPossibleBreakpointLocations() {
            return debugSession.GetPossibleBreakpointLocations();
        }

        public void Continue() {
            debugSession.Continue();
        }

        public void StepOver() {
            debugSession.StepOver();
        }

        public void StepIn() {
            debugSession.StepIn();
        }

        public void StepOut() {
            debugSession.StepOut();
        }

        public void Pause() {
            debugSession.RequestPause(PauseReasonType.Pause);
        }

        public void Terminate() {
            debugSession.RequestTerminate();
            ExitedEvent(0);

            if (!debugThread.Join(3000)) {
                OutputEvent(OutputType.Stderr, "Could not terminate - termination time exceeded");
            }

            TerminatedEvent();
        }

        public void Disconnect() {
            debugSession.RequestTerminate();
            debugThread.Join(3000);
        }

        public class Factory {
            public static DebugSessionController Create(KPC8Configuration kpcConfig, DebugSessionConfiguration debugSessionConfig) {
                var kpcBuild = new KPC8Builder(kpcConfig).Build();
                return new DebugSessionController(debugSessionConfig, kpcBuild);
            }
        }
    }
}

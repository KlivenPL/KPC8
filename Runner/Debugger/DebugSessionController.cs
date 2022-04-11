using Runner.Build;
using Runner.Configuration;
using Runner.Debugger.DebugData;
using Runner.Debugger.Enums;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Runner.Debugger {
    public class DebugSessionController {

        private readonly KPC8Build kpc;
        private readonly DebugSession debugSession;
        private readonly DebugSessionConfiguration configuration;

        private readonly object syncObject;
        private readonly ManualResetEventSlim runEvent;

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

        internal DebugSessionController(DebugSessionConfiguration configuration, KPC8Build kpc) {
            syncObject = new object();
            runEvent = new ManualResetEventSlim();
            debugSession = new DebugSession(configuration, kpc, runEvent, syncObject);
            this.configuration = configuration;
            this.kpc = kpc;

            SubscribeToPassedEvents();
        }

        private void SubscribeToPassedEvents() {
            debugSession.InitializedEvent += InitializedEvent;
            debugSession.InvalidatedEvent += InvalidatedEvent;
            debugSession.OutputEvent += OutputEvent;
            debugSession.PausedEvent += PausedEvent;
        }

        internal void StartDebugging() {
            debugThread = new Thread(debugSession.Initialize);
            debugThread.Name = "Main debug thread";
            debugThread.Start();
        }

        public IEnumerable<BreakpointInfo> GetPossibleBreakpointLocations() {
            return debugSession.GetPossibleBreakpointLocations();
        }

        public void Continue() {

        }

        public void StepOver() {

        }

        public void StepIn() {

        }

        public void StepOut() {

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
    }
}

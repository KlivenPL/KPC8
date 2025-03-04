using KPC8.ProgRegs;
using Runner._Infrastructure;
using Runner.Build;
using Runner.Configuration;
using Runner.Debugger.DebugData;
using Runner.Debugger.Enums;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Runner.Debugger {
    public class DebugSessionController : IKPC8SessionController {
        private readonly KPC8Build kpc;

        private readonly object syncObject;
        private readonly ManualResetEventSlim runEvent;

        private readonly DebugSession debugSession;
        private readonly CancellationTokenSource cts;
        private Thread debugThread;

        #region DebugSessionController Events

        public event Action<int> ExitedEvent;
        public event Action TerminatedEvent;

        #endregion

        #region DebugSession passed Events

        public event Action<DebugInfo> InvalidatedEvent;
        public event Action<OutputType, string> OutputEvent;
        public event Action<PauseReasonType, DebugInfo> PausedEvent;

        #endregion

        public bool IsStarted => debugThread?.IsAlive == true;

        KPC8Build IKPC8SessionController.GetKPC8Build => kpc;

        private DebugSessionController(DebugSessionConfiguration configuration, KPC8Build kpc) {
            this.kpc = kpc;
            syncObject = new object();
            runEvent = new ManualResetEventSlim(true);
            debugSession = new DebugSession(configuration, kpc, runEvent, syncObject);
            cts = new CancellationTokenSource();
        }

        public void StartDebugging(bool pauseAtEntry) {
            SubscribeToPassedEvents();
            debugThread = new Thread(() => debugSession.Start(pauseAtEntry, cts.Token));
            debugThread.Name = "Main KPC8 debug thread";
            debugThread.Priority = ThreadPriority.Highest;
            debugThread.Start();
        }

        private void SubscribeToPassedEvents() {
            debugSession.InvalidatedEvent += InvalidatedEvent;
            debugSession.OutputEvent += OutputEvent;
            debugSession.PausedEvent += PausedEvent;
        }

        public IEnumerable<BreakpointInfo> GetPossibleBreakpointLocations() {
            return debugSession.GetPossibleBreakpointLocations();
        }

        public IEnumerable<BreakpointInfo> SetBreakpoints(IEnumerable<(string filePath, int line, int column)> proposedBreakpoints) {
            return debugSession.SetBreakpoints(proposedBreakpoints);
        }

        public byte[] GetRamBytes() {
            return debugSession.GetRamBytes();
        }

        public void SetRamBytes(ushort address, byte value) {
            debugSession.SetRamByte(address, value);
        }

        public byte[] GetRomBytes() {
            return debugSession.GetRomBytes();
        }

        public void SetRegister(Regs register, ushort value) {
            debugSession.SetRegister(register, value);
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
            debugSession.RequestPause();
        }

        public void ChangeDebugValueFormat(DebugValueFormat newFormat) {
            debugSession.ChangeDebugValueFormat(newFormat);
        }

        public void Terminate() {
            cts.Cancel();
            debugSession.RequestTerminate();
            ExitedEvent(0);

            if (!debugThread.Join(5000)) {
                OutputEvent(OutputType.Stderr, "Could not terminate - termination time exceeded");
            }

            TerminatedEvent();
        }

        public void Disconnect() {
            cts.Cancel();
            debugSession.RequestTerminate();
            debugThread?.Join(5000);
            TerminatedEvent?.Invoke();
        }

        public class Factory {
            public static DebugSessionController Create(KPC8Configuration kpcConfig, DebugSessionConfiguration debugSessionConfig) {
                var kpcBuild = new KPC8Builder(kpcConfig).Build();
                return new DebugSessionController(debugSessionConfig, kpcBuild);
            }
        }
    }
}

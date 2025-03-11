using Abstract;
using KPC8.ProgRegs;
using LightweightEmulator.Configuration;
using LightweightEmulator.Pipelines;
using Runner._Infrastructure;
using Runner.Build;
using Runner.Configuration;
using Runner.Debugger.DebugData;
using Runner.Debugger.Enums;
using Runner.EmulationControl;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Runner.Debugger {
    public class DebugSessionController : IKPC8SessionController {
        private readonly IKpcBuild kpc;
        private readonly IEmulationController emulationController;

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

        IKpcBuild IKPC8SessionController.GetKPC8Build => kpc;

        private DebugSessionController(
            DebugSessionConfiguration configuration,
            IKpcBuild kpc,
            IEmulationController emulationController) {

            this.kpc = kpc;
            this.emulationController = emulationController;
            syncObject = new object();
            runEvent = new ManualResetEventSlim(true);
            debugSession = new DebugSession(configuration, kpc, emulationController, runEvent, syncObject);
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

        public IEnumerable<BreakpointInfo> SetBreakpoints(string filePath, IEnumerable<(int line, int column)> proposedBreakpoints) {
            return debugSession.SetBreakpoints(filePath, proposedBreakpoints);
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
                var emulationController = new KPC8EmulationController(kpcBuild);
                return new DebugSessionController(debugSessionConfig, kpcBuild, emulationController);
            }

            public static DebugSessionController CreateLw(LwKpcConfiguration kpcConfig, DebugSessionConfiguration debugSessionConfig) {
                var kpcBuild = new LwKpcBuilder(kpcConfig).Build();
                var emulationController = new LwEmulationController(kpcBuild);
                return new DebugSessionController(debugSessionConfig, kpcBuild, emulationController);
            }
        }
    }
}

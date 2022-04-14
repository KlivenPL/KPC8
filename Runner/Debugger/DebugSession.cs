using _Infrastructure.BitArrays;
using Infrastructure.BitArrays;
using KPC8.ControlSignals;
using KPC8.CpuFlags;
using KPC8.ProgRegs;
using Runner._Infrastructure;
using Runner.Build;
using Runner.Configuration;
using Runner.Debugger.DebugData;
using Runner.Debugger.Enums;
using Runner.Debugger.Managers;
using Simulation.Loops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Runner.Debugger {
    internal class DebugSession {

        private readonly DebugSessionConfiguration configuration;
        private readonly ManualResetEventSlim runEvent;
        private readonly object syncObject;
        private readonly KPC8Build kpc;

        private readonly BreakpointManager breakpointManager;

        private PauseReasonType? pauseReason;
        private bool paused = false;
        private ushort? nextPauseAddress = null;
        private int? hitBreakpointId = null;
        private bool terminate = false;
        private DebugValueFormat debugValueFormat = DebugValueFormat.DecWordUnsigned;
        private List<SimulationLoopRunner> externalSlRunners;

        #region DebugSession Events

        internal event Action<DebugInfo> InvalidatedEvent;
        internal event Action<OutputType, string> OutputEvent;
        internal event Action<PauseReasonType, DebugInfo> PausedEvent;

        #endregion

        internal DebugSession(DebugSessionConfiguration configuration, KPC8Build kpc, ManualResetEventSlim runEvent, object syncObject) {
            this.runEvent = runEvent;
            this.kpc = kpc;
            this.syncObject = syncObject;
            this.configuration = configuration;

            breakpointManager = new BreakpointManager(configuration.DebugSymbols);
            externalSlRunners = new List<SimulationLoopRunner>();
        }

        internal void Start() {
            if (configuration.StopAtEntry) {
                RequestPause();
            }

            foreach (var externalModuleSl in kpc.ExternalSimulationLoops) {
                externalSlRunners.Add(SimulationLoopRunner.RunInNewThread(externalModuleSl));
            }

            // Load the first instruction
            MakeTickAndWait();
            MakeTickAndWait();
            MakeTickAndWait();

            DebuggerLoop();
        }

        private void DebuggerLoop() {
            do {
                if (!paused) {
                    var pcCurrInstrAddress = (ushort)(kpc.ModulePanel.Memory.PcContent.ToUShortLE() - 1);

                    if (breakpointManager.IsBreakpointHit(pcCurrInstrAddress, out hitBreakpointId)) {
                        // If a breakpoint is encountered, send a stopped event
                        OutputEvent(OutputType.Stdout, $"BP hit at address: {pcCurrInstrAddress} bpid: {hitBreakpointId}\n");
                        RequestPause(PauseReasonType.Breakpoint);
                    } else if (pauseReason == PauseReasonType.Step &&
                            breakpointManager.CanPauseHere(pcCurrInstrAddress, out hitBreakpointId) &&
                            (nextPauseAddress == null || nextPauseAddress == pcCurrInstrAddress)) {

                        OutputEvent(OutputType.Stdout, $"STEP hit at address: {pcCurrInstrAddress} bpid: {hitBreakpointId}\n");
                        RequestPause(PauseReasonType.Step);
                    }
                }

                lock (syncObject) {
                    if (!runEvent.IsSet) {
                        HandlePause();
                    }
                }

                runEvent.Wait();
                TickOneInstruction();

            } while (!terminate);

            foreach (var externalSlRunner in externalSlRunners) {
                externalSlRunner.Kill();
            }
        }

        private void TickOneInstruction() {
            // Execute currently loaded instruction
            while (!kpc.CsPanel.Ctrl.Ic_clr) {
                MakeTickAndWait();
            }

            /*while (kpc.CsPanel.Ctrl.Ic_clr) {
                MakeTickAndWait();
            }*/

            // Load next instruction
            MakeTickAndWait();
            MakeTickAndWait();
            MakeTickAndWait();
            MakeTickAndWait();
        }

        private void MakeTickAndWait() {
            kpc.MainSimulationLoop.Loop();

            kpc.MainClock.MakeTick();

            while (kpc.MainClock.IsManualTickInProgress) {
                kpc.MainSimulationLoop.Loop();
            }

            kpc.MainSimulationLoop.Loop();
        }

        private void HandlePause() {
            if (!pauseReason.HasValue) {
                throw new InvalidOperationException("Stopping for no reason!");
            }
            paused = true;
            PausedEvent(pauseReason.Value, GetDebugInfo());
            pauseReason = null;
        }

        private void Continue(bool step, ushort? nextPauseAddress) {
            lock (syncObject) {
                if (step) {
                    pauseReason = PauseReasonType.Step;
                    this.nextPauseAddress = nextPauseAddress;
                } else {
                    pauseReason = null;
                    this.nextPauseAddress = null;
                }
            }

            paused = false;
            runEvent.Set();
        }

        private DebugInfo GetDebugInfo() {
            if (!paused) {
                OutputEvent(OutputType.Stderr, "Cannot get data when debugger is not paused");
                return null;
            }

            DebugInfo data = new DebugInfo {
                HitBreakpointId = hitBreakpointId,
                Frames = new StackFrameInfo[] {
                    new StackFrameInfo {
                        Line = breakpointManager.GetLineOfBreakpoint(hitBreakpointId),
                        Scopes = new ScopeInfo [] {
                            new ScopeInfo {
                                Name = "Registers",
                                VariablesReference = 1,
                                Variables = GetRegisters(),
                            },
                            new ScopeInfo {
                                Name = "Internal registers",
                                VariablesReference = 2,
                                Variables = GetInternalRegisters().ToArray(),
                            }
                        }
                    }
                }
            };

            return data;

            VariableInfo[] GetRegisters() {
                var registers = Enum.GetValues<Regs>().Where(r => r != Regs.None).Select(x => {
                    var content = kpc.ModulePanel.Registers.GetWholeRegContent(x.GetIndex());

                    return new VariableInfo {
                        Name = x.ToString(),
                        Value = content.ToFormattedDebugString(debugValueFormat)
                    };
                });

                return registers.ToArray();
            }

            IEnumerable<VariableInfo> GetInternalRegisters() {
                yield return new VariableInfo {
                    Name = "PC",
                    Value = kpc.ModulePanel.Memory.PcContent.ToFormattedDebugString(debugValueFormat),
                };

                yield return new VariableInfo {
                    Name = "MAR",
                    Value = kpc.ModulePanel.Memory.MarContent.ToFormattedDebugString(debugValueFormat),
                };

                yield return new VariableInfo {
                    Name = "Flags",
                    Value = CpuFlagExtensions.From8BitArray(BitArrayHelper.FromByteLE(0).Take(4).MergeWith(kpc.ModulePanel.FlagsBus.Lanes.ToBitArray())).ToString()
                };
            }
        }

        private void RequestPause(PauseReasonType pauseReason) {
            lock (syncObject) {
                this.pauseReason = pauseReason;
                runEvent.Reset();
            }
        }

        #region Potentially external thread section

        internal void ChangeDebugValueFormat(DebugValueFormat newFormat) {
            debugValueFormat = newFormat;
            InvalidatedEvent(GetDebugInfo());
        }

        internal IEnumerable<BreakpointInfo> GetPossibleBreakpointLocations() {
            return breakpointManager.GetPossibleBreakpointLocations();
        }

        /// <returns>Successfully placed breakpoints</returns>
        internal IEnumerable<BreakpointInfo> SetBreakpoints(IEnumerable<(int line, int column)> proposedBreakpoints) {
            //OutputEvent(OutputType.Stdout, "SET BRP: " + breakpointManager);
            return breakpointManager.SetBreakpoints(proposedBreakpoints);
        }

        internal void Continue() {
            Continue(false, null);
        }

        internal void StepOver() {
            var pcCurrInstrAddress = (ushort)(kpc.ModulePanel.Memory.PcContent.ToUShortLE() - 1);
            Continue(true, breakpointManager.GetNextPossibleBreakpointAddressInAddressOrder(pcCurrInstrAddress));
        }

        internal void StepIn() {
            Continue(true, null);
        }

        internal void StepOut() {
            Continue(true, null);
        }

        internal void RequestPause() {
            Continue(true, null);
        }

        internal void RequestTerminate() {
            lock (syncObject) {
                terminate = true;
            }
        }

        #endregion
    }
}

using _Infrastructure.Paths;
using Abstract;
using Assembler.DebugData;
using Infrastructure.BitArrays;
using KPC8.CpuFlags;
using KPC8.ProgRegs;
using Runner._Infrastructure;
using Runner.Configuration;
using Runner.Debugger.DebugData;
using Runner.Debugger.Enums;
using Runner.Debugger.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Runner.Debugger {
    internal class DebugSession {

        private readonly DebugSessionConfiguration configuration;
        private readonly ManualResetEventSlim runEvent;
        private readonly object syncObject;
        private readonly IKpcBuild kpc;
        private readonly IEmulationController emulationController;

        private readonly BreakpointManager breakpointManager;
        private readonly ConstantValuesManager constantValuesManager;
        private readonly DebugWriteManager debugWriteManager;

        private PauseReasonType? pauseReason;
        private bool paused = false;
        private ushort? nextPauseAddress = null;
        private int? hitBreakpointId = null;
        private bool terminate = false;
        private DebugValueFormat debugValueFormat = DebugValueFormat.DecWordUnsigned;

        #region DebugSession Events

        internal event Action<DebugInfo> InvalidatedEvent;
        internal event Action<OutputType, string> OutputEvent;
        internal event Action<PauseReasonType, DebugInfo> PausedEvent;

        #endregion

        internal DebugSession(DebugSessionConfiguration configuration, IKpcBuild kpc, IEmulationController emulationController, ManualResetEventSlim runEvent, object syncObject) {
            this.runEvent = runEvent;
            this.kpc = kpc;
            this.syncObject = syncObject;
            this.configuration = configuration;

            breakpointManager = new BreakpointManager(configuration.DebugSymbols);
            constantValuesManager = new ConstantValuesManager(configuration.DebugSymbols);
            debugWriteManager = new DebugWriteManager(configuration.DebugSymbols);
            this.emulationController = emulationController;
        }

        internal void Start(bool pauseAtEntry, CancellationToken cancellationToken) {
            if (pauseAtEntry) {
                RequestPause();
            }

            emulationController.InitializeDebug();

            DebuggerLoop(cancellationToken);
        }

        private void DebuggerLoop(CancellationToken cancellationToken) {
            do {
                if (!paused) {
                    var pcCurrInstrAddress = (ushort)(kpc.Pc.WordValue + 1);

                    if (debugWriteManager.IsDebugWriteHit((ushort)(pcCurrInstrAddress + 2), out var debugWrites)) {
                        var allRegisters = GetRegisters().Concat(GetInternalRegisters());
                        HandleDebugWrite(debugWrites, allRegisters, constantValuesManager.GetValues(allRegisters).OrderByDescending(x => x.Line));
                    }

                    if (breakpointManager.IsBreakpointHit(pcCurrInstrAddress, out hitBreakpointId)) {
                        // If a breakpoint is encountered, send a stopped event
                        // OutputEvent(OutputType.Stdout, $"BP hit at address: {pcCurrInstrAddress} bpid: {hitBreakpointId}\n");
                        RequestPause(PauseReasonType.Breakpoint);
                    } else if (pauseReason == PauseReasonType.Step &&
                            breakpointManager.CanPauseHere(pcCurrInstrAddress, out hitBreakpointId) &&
                            (nextPauseAddress == null || nextPauseAddress == pcCurrInstrAddress)) {

                        // OutputEvent(OutputType.Stdout, $"STEP hit at address: {pcCurrInstrAddress} bpid: {hitBreakpointId}\n");
                        RequestPause(PauseReasonType.Step);
                    }
                }

                lock (syncObject) {
                    if (!runEvent.IsSet) {
                        HandlePause();
                    }
                }

                try {
                    runEvent.Wait(cancellationToken);
                } catch (OperationCanceledException) {
                    terminate = true;
                    continue;
                }

                emulationController.ExecuteSingleInstruction();

            } while (!terminate);

            emulationController.Terminate();
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

            var registersVariableInfos = GetRegisters();
            var internalRegistersVariableInfos = GetInternalRegisters().ToArray();
            breakpointManager.GetBreakpointData(hitBreakpointId, out var filePath, out var line);

            DebugInfo data = new DebugInfo {
                HitBreakpointId = hitBreakpointId,
                ConstantValues = constantValuesManager.GetValues(registersVariableInfos.Concat(internalRegistersVariableInfos)).OrderByDescending(x => x.Line),
                Frames = new StackFrameInfo[] {
                    new StackFrameInfo {
                        Line = line,
                        FilePath = filePath,
                        Scopes = new ScopeInfo [] {
                            new ScopeInfo {
                                Name = "Registers",
                                VariablesReference = 1,
                                Variables = registersVariableInfos,
                            },
                            new ScopeInfo {
                                Name = "Internal registers",
                                VariablesReference = 2,
                                Variables = internalRegistersVariableInfos,
                            }
                        }
                    }
                }
            };

            return data;
        }

        private VariableInfo[] GetRegisters() {
            var registers = Enum.GetValues<Regs>().Where(r => r != Regs.None).Select(x => {
                var content = kpc.ProgrammerRegisters[x.GetIndex()].WordValue;

                return new VariableInfo {
                    Name = x.ToString(),
                    Value = BitArrayHelper.FromUShortLE(content).ToFormattedDebugString(debugValueFormat),
                    ValueRaw = content,
                };
            });

            return registers.ToArray();
        }

        private IEnumerable<VariableInfo> GetInternalRegisters() {
            yield return new VariableInfo {
                Name = "PC",
                Value = BitArrayHelper.FromUShortLE(kpc.Pc.WordValue).ToFormattedDebugString(debugValueFormat),
                MemoryReference = "ROM",
                ValueRaw = kpc.Pc.WordValue,
            };

            yield return new VariableInfo {
                Name = "MAR",
                Value = BitArrayHelper.FromUShortLE(kpc.Mar.WordValue).ToFormattedDebugString(debugValueFormat),
                MemoryReference = "RAM",
                ValueRaw = kpc.Mar.WordValue,
            };

            yield return new VariableInfo {
                Name = "Flags",
                Value = ((CpuFlag)kpc.Flags.Value).ToString()
            };
        }

        private void HandleDebugWrite(IEnumerable<DebugWriteSymbol> debugWrites, IEnumerable<VariableInfo> allRegisterInfos, IEnumerable<ConstantValueInfo> constantValues) {

            foreach (var (debugMessage, filePath, line) in debugWriteManager.GetValues(debugWrites, TryGetRegister, TryGetConstant, TryEvaluateExpression)) {
                OutputEvent?.Invoke(OutputType.Stdout, $"[DebugWrite {Path.GetFileName(filePath)}:{line}] {debugMessage}\n");
            }

            bool TryGetRegister(string name, out string value) {
                value = allRegisterInfos.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase))?.Value;
                return value != null;
            }

            bool TryGetConstant(string name, string filePath, int line, out string value) {
                value = constantValues.FirstOrDefault(x => x.FilePath.ComparePath(filePath) && x.Name == name && x.Line <= line + 1)?.Value;
                return value != null;
            }

            bool TryEvaluateExpression(string expression, string filePath, int line, out string value) {
                if (expression.Equals("time", StringComparison.OrdinalIgnoreCase)) {
                    value = DateTime.Now.ToString("HH:mm:ss:fff");
                    return true;
                }

                var parameters = GetAllParameters(expression, filePath, line).ToArray();

                if (expression.StartsWith("ram", StringComparison.OrdinalIgnoreCase)) {
                    if (parameters.Length != 1) {
                        value = "Wrong usage. Examples: RAM($sp), RAM(registerAlias), RAM(constantAlias), RAM(0x2137)";
                        return false;
                    }
                    value = BitArrayHelper.FromByteLE(kpc.Ram.ReadByte(parameters[0])).ToFormattedDebugString8Bit(debugValueFormat);
                    return true;
                }

                if (expression.StartsWith("rom", StringComparison.OrdinalIgnoreCase)) {
                    if (parameters.Length != 1) {
                        value = "Wrong usage. Examples: ROM($sp), ROM(registerAlias), ROM(constantAlias), ROM(0x2137)";
                        return false;
                    }
                    value = BitArrayHelper.FromByteLE(kpc.Rom.ReadByte(parameters[0])).ToFormattedDebugString8Bit(debugValueFormat);
                    return true;
                }

                value = "Unknown expression";
                return false;

                IEnumerable<ushort> GetAllParameters(string str, string filePath, int line) {
                    var matches = Regex.Matches(str, @"\((.*?)\)");

                    foreach (Match match in matches.Where(m => m.Success && m.Groups.Count == 2)) {
                        var split = match.Groups[1].Value.Split(',', StringSplitOptions.RemoveEmptyEntries);

                        foreach (var arg in split) {
                            if (TryGetValueFromRegisterOrConstant(arg, filePath, line, out var result)) {
                                yield return result;
                            }
                        }
                    }
                }

                bool TryGetValueFromRegisterOrConstant(string value, string filePath, int line, out ushort result) {
                    if (value[0] == '-') {
                        if (short.TryParse(value, out var signedResult)) {
                            result = (ushort)signedResult;
                            return true;
                        }
                    }

                    if (value.StartsWith("0x")) {
                        if (ushort.TryParse(value[2..], System.Globalization.NumberStyles.AllowHexSpecifier, null, out var unsignedHexResult)) {
                            result = unsignedHexResult;
                            return true;
                        }
                    }

                    if (value.StartsWith("0b")) {
                        if (TryBinToDec(value[2..], out var binaryResult)) {
                            result = binaryResult;
                            return true;
                        }
                    }

                    if (ushort.TryParse(value, out var unsignedResult)) {
                        result = unsignedResult;
                        return true;
                    }

                    if (value.StartsWith('$')) {
                        return TryGetRegister(value[1..], out result);
                    }

                    return TryGetConstant(value, filePath, line, out result);
                }

                bool TryGetRegister(string name, out ushort value) {
                    value = 0;
                    var posValue = allRegisterInfos.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase))?.ValueRaw;
                    value = posValue ?? 0;
                    return posValue.HasValue;
                }

                bool TryGetConstant(string name, string filePath, int line, out ushort value) {
                    value = 0;
                    var posValue = constantValues.FirstOrDefault(x => x.FilePath.ComparePath(filePath) && x.Name == name && x.Line <= line + 1)?.ValueRaw;
                    value = posValue ?? 0;
                    return posValue.HasValue;
                }
            }
        }

        private bool TryBinToDec(string val, out ushort result) {
            result = 0;

            if (val.Length == 0)
                return false;

            var tmpSum = 0;

            for (int i = val.Length - 1; i >= 0; i--) {
                if (val[i] == '0' || val[i] == '1') {
                    var numVal = int.Parse(val[i].ToString());
                    tmpSum += numVal == 1 ? 1 << val.Length - i - 1 : 0;
                } else {
                    return false;
                }
            }

            if (tmpSum >= 0 && tmpSum <= ushort.MaxValue) {
                result = (ushort)tmpSum;
                return true;
            }

            return false;
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
        internal IEnumerable<BreakpointInfo> SetBreakpoints(string filePath, IEnumerable<(int line, int column)> proposedBreakpoints) {
            //OutputEvent(OutputType.Stdout, "SET BRP: " + breakpointManager);
            return breakpointManager.SetBreakpoints(filePath, proposedBreakpoints);
        }

        internal byte[] GetRamBytes() {
            if (!paused) {
                OutputEvent(OutputType.Stderr, "Cannot get RAM bytes if not paused");
                return null;
            }

            return kpc.Ram.DumpToBytes();
        }

        internal void SetRamByte(ushort address, byte value) {
            if (!paused) {
                OutputEvent(OutputType.Stderr, "Cannot set RAM bytes if not paused");
                return;
            }

            kpc.Ram.WriteByte(value, address);
        }

        internal byte[] GetRomBytes() {
            if (!paused) {
                OutputEvent(OutputType.Stderr, "Cannot get ROM bytes if not paused");
                return null;
            }

            return kpc.Rom.DumpToBytes();
        }

        internal void SetRegister(Regs register, ushort value) {
            if (!paused) {
                OutputEvent(OutputType.Stderr, "Cannot set register if not paused");
                return;
            }

            kpc.ProgrammerRegisters[register.GetIndex()].WordValue = value;
        }

        internal void Continue() {
            Continue(false, null);
        }

        internal void StepOver() {
            var pcCurrInstrAddress = (ushort)(kpc.Pc.WordValue + 1);
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

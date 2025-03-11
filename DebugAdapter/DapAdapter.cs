using _Infrastructure.Paths;
using DebugAdapter.Configuration;
using DebugAdapter.CustomRequests;
using DebugAdapter.Mappers;
using KPC8.ProgRegs;
using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol;
using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Messages;
using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Utilities;
using Runner.Debugger;
using Runner.Debugger.DebugData;
using Runner.Debugger.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DebugAdapter {
    public class DapAdapter : DebugAdapterBase {
        private const string RomMemoryReference = "ROM";
        private const string RamMemoryReference = "RAM";

        private readonly DapAdapterConfiguration configuration;
        private readonly DebugSessionController sessionController;
        private readonly IEnumerable<Source> sources;
        //private readonly IEnumerable<string> filePaths;

        private DebugInfo debugInfo;
        private bool pauseAtEntry;

        private int pauseId;
        private int cachedRamPauseId;
        private byte[] cachedRamBytes;
        private byte[] cachedRomBytes;

        public DapAdapter(DapAdapterConfiguration configuration, DebugSessionController sessionController, Stream stdIn, Stream stdOut) {
            this.configuration = configuration;
            this.sessionController = sessionController;
            sources = configuration?.SourceFilePaths.Select(x => new Source { Path = x }).ToList();

            InitializeProtocolClient(stdIn, stdOut);
        }

        private void SubscribeToEvents() {
            sessionController.OutputEvent += SessionController_OutputEvent;
            sessionController.PausedEvent += SessionController_PausedEvent;
            sessionController.InvalidatedEvent += SessionController_InvalidatedEvent;
            sessionController.ExitedEvent += SessionController_ExitedEvent;
            sessionController.TerminatedEvent += SessionController_TerminatedEvent;
        }

        public void Run() {
            SubscribeToEvents();
            Protocol.Run();
        }

        public void RunAndExitWithErrorMessage(string errorMessage) {
            Protocol.Run();
            SendOutput(OutputType.Stderr, errorMessage);
            Protocol.Stop();
        }

        protected override InitializeResponse HandleInitializeRequest(InitializeArguments arguments) {
            ReqisterCustomRequests();

            return new InitializeResponse {
                SupportsConfigurationDoneRequest = true,
                SupportsEvaluateForHovers = true,
                SupportsReadMemoryRequest = true,
                SupportsWriteMemoryRequest = true,
                SupportsSetVariable = true,
                //SupportsBreakpointLocationsRequest = true,
            };
        }

        private void ReqisterCustomRequests() {
            Protocol.RegisterRequestType<ChangeFormatRequest, ChangeFormatRequestArguments>(toggleFormatting => {
                sessionController.ChangeDebugValueFormat(toggleFormatting.Arguments.Format);
                Protocol.SendEvent(new InvalidatedEvent());
            });
        }

        protected override SetBreakpointsResponse HandleSetBreakpointsRequest(SetBreakpointsArguments arguments) {

            /*Console.WriteLine("protocol path: " + arguments.Source.Path);
            Console.WriteLine("args path: " + source.Path);
            Console.WriteLine("are equal: " + ComparePaths(arguments.Source.Path, this.source.Path));*/

            // PRZYWROCOC
            if (!PathComparer.Contains(arguments.Source.Path, configuration.SourceFilePaths)) {
                return new SetBreakpointsResponse {
                    Breakpoints = arguments.Breakpoints.Select(x => x.ToUnverifiedBreakpoint(arguments.Source.Path)).ToList()
                };
            }

            /*if (!arguments.Source.Path.EndsWith(".kpc")) {
                return new SetBreakpointsResponse {
                    Breakpoints = arguments.Breakpoints.Select(x => x.ToUnverifiedBreakpoint()).ToList()
                };

            }*/

            var possibleBreakpoints = sessionController.GetPossibleBreakpointLocations();

            /*var tmpVerified = arguments.Breakpoints.Where(abp => possibleBreakpoints.Any(pb => pb.Line == abp.Line && pb.Column == abp.Column));
            var unverified = arguments.Breakpoints.Except(tmpVerified);
            var verified = possibleBreakpoints.Where(pb => arguments.Breakpoints.Any(abp => pb.Line == abp.Line && pb.Column == abp.Column));*/

            List<Breakpoint> resultBps = new List<Breakpoint>(arguments.Breakpoints.Count);
            List<(int line, int column)> acceptedBreakpoints = new List<(int line, int column)>();

            foreach (var abp in arguments.Breakpoints) {
                var acceptedBp = possibleBreakpoints.FirstOrDefault(pb => pb.FilePath.ComparePath(arguments.Source.Path) && pb.Line == abp.Line && (pb.Column == abp.Column || abp.Column == null));
                if (acceptedBp != null) {
                    resultBps.Add(acceptedBp.ToVerifiedBreakpoint());
                    acceptedBreakpoints.Add((acceptedBp.Line, acceptedBp.Column));
                } else {
                    resultBps.Add(abp.ToUnverifiedBreakpoint(arguments.Source.Path));
                }
            }

            var count = sessionController.SetBreakpoints(possibleBreakpoints.First(
                x => x.FilePath.ComparePath(arguments.Source.Path)).FilePath,
                acceptedBreakpoints).Count();

            Console.WriteLine($"setbp count: {count}");
            Console.WriteLine($"count: {acceptedBreakpoints.Count}");

            /*if (count != acceptedBreakpoints.Count) {
                throw new ProtocolException();
            }*/

            return new SetBreakpointsResponse {
                Breakpoints = resultBps,
            };
        }

        protected override BreakpointLocationsResponse HandleBreakpointLocationsRequest(BreakpointLocationsArguments arguments) {
            return new BreakpointLocationsResponse {
                Breakpoints = sessionController.GetPossibleBreakpointLocations().Select(x => new BreakpointLocation { Column = x.Column, EndColumn = x.EndColumn, Line = x.Line }).ToList()
            };
        }

        protected override SetExceptionBreakpointsResponse HandleSetExceptionBreakpointsRequest(SetExceptionBreakpointsArguments arguments) {
            return new SetExceptionBreakpointsResponse();
        }

        protected override ConfigurationDoneResponse HandleConfigurationDoneRequest(ConfigurationDoneArguments arguments) {
            if (!sessionController.IsStarted) {
                sessionController.StartDebugging(pauseAtEntry);
            }

            return new ConfigurationDoneResponse();
        }

        protected override AttachResponse HandleAttachRequest(AttachArguments arguments) {
            pauseAtEntry = arguments.ConfigurationProperties.GetValueAsBool("pauseAtEntry") ?? false;
            Protocol.SendEvent(new InitializedEvent());
            return new AttachResponse();
        }

        protected override LaunchResponse HandleLaunchRequest(LaunchArguments arguments) {
            /*string sourceFilePath = arguments.ConfigurationProperties.GetValueAsString("sourceFilePath");

            if (string.IsNullOrEmpty(sourceFilePath)) {
                throw new ProtocolException("Launch failed because launch configuration did not specify 'sourceFilePath'.");
            }

            if (!File.Exists(sourceFilePath)) {
                throw new ProtocolException($"Could not find the source file at: \"{sourceFilePath}\"");
            }*/

            Protocol.SendEvent(new InitializedEvent());

            return new LaunchResponse();
        }


        protected override NextResponse HandleNextRequest(NextArguments arguments) {
            sessionController.StepOver();
            return new NextResponse();
        }

        protected override ContinueResponse HandleContinueRequest(ContinueArguments arguments) {
            sessionController.Continue();
            return new ContinueResponse();
        }

        protected override StepInResponse HandleStepInRequest(StepInArguments arguments) {
            sessionController.StepIn();
            return new StepInResponse();
        }

        protected override StepOutResponse HandleStepOutRequest(StepOutArguments arguments) {
            sessionController.StepOut();
            return new StepOutResponse();
        }

        protected override PauseResponse HandlePauseRequest(PauseArguments arguments) {
            sessionController.Pause();
            return new PauseResponse();
        }

        protected override TerminateResponse HandleTerminateRequest(TerminateArguments arguments) {
            sessionController.Terminate();
            return new TerminateResponse();
        }

        protected override DisconnectResponse HandleDisconnectRequest(DisconnectArguments arguments) {
            sessionController.Disconnect();
            return new DisconnectResponse();
        }

        protected override ThreadsResponse HandleThreadsRequest(ThreadsArguments arguments) {
            return new ThreadsResponse {
                Threads = new() {
                    new() {
                        Id = 0,
                        Name = "Main thread"
                    }
                }
            };
        }

        protected override StackTraceResponse HandleStackTraceRequest(StackTraceArguments arguments) {
            return new StackTraceResponse {
                StackFrames = debugInfo.Frames
                    .Select(x => x.ToStackFrame(arguments))
                    .ToList()
            };
        }

        protected override ScopesResponse HandleScopesRequest(ScopesArguments arguments) {
            return new ScopesResponse {
                Scopes = debugInfo.Frames
                    .First().Scopes
                    .Select(s => s.ToScope())
                    .ToList()
            };
        }

        protected override VariablesResponse HandleVariablesRequest(VariablesArguments arguments) {
            return new VariablesResponse {
                Variables = debugInfo.Frames
                    .First().Scopes
                    .First(x => x.VariablesReference == arguments.VariablesReference).Variables
                    .Select(x => x.ToProtocolVariable())
                    .ToList()
            };
        }

        protected override SetVariableResponse HandleSetVariableRequest(SetVariableArguments arguments) {
            if (!Enum.TryParse<Regs>(arguments.Name, out var register) || register == Regs.None || register == Regs.Zero || !ushort.TryParse(arguments.Value, out var value)) {
                return new SetVariableResponse { };
            }

            sessionController.SetRegister(register, value);

            return new SetVariableResponse {
                Value = value.ToString()
            };
        }

        protected override ReadMemoryResponse HandleReadMemoryRequest(ReadMemoryArguments arguments) {
            if (arguments.MemoryReference == RamMemoryReference) {
                if (cachedRamPauseId != pauseId) {
                    cachedRamPauseId = pauseId;
                    cachedRamBytes = sessionController.GetRamBytes();
                }

                return new ReadMemoryResponse {
                    Address = arguments.Offset?.ToString() ?? "0",
                    Data = Convert.ToBase64String(cachedRamBytes.Skip(arguments.Offset ?? 0).Take(arguments.Count).ToArray()),
                    UnreadableBytes = 0
                };
            } else if (arguments.MemoryReference == RomMemoryReference) {
                cachedRomBytes ??= sessionController.GetRomBytes();

                return new ReadMemoryResponse {
                    Address = arguments.Offset?.ToString() ?? "0",
                    Data = Convert.ToBase64String(cachedRomBytes.Skip(arguments.Offset ?? 0).Take(arguments.Count).ToArray()),
                    UnreadableBytes = 0
                };

            } else {

                return new ReadMemoryResponse {
                    Address = "0x0",
                    Data = "",
                    UnreadableBytes = 0
                };
            }
        }

        protected override WriteMemoryResponse HandleWriteMemoryRequest(WriteMemoryArguments arguments) {
            if (arguments.MemoryReference != RamMemoryReference) {
                return new WriteMemoryResponse {
                    BytesWritten = 0,
                    Offset = 0,
                };
            }

            var rawData = Convert.FromBase64String(arguments.Data);
            if (rawData.Length == 0 || rawData.Length > ushort.MaxValue) {
                return new WriteMemoryResponse {
                    BytesWritten = 0,
                    Offset = 0,
                };
            }

            var offset = (ushort)(arguments.Offset ?? 0);

            for (ushort i = 0; i < rawData.Length; i++) {
                sessionController.SetRamBytes((ushort)(i + offset), rawData[i]);
            }

            cachedRamPauseId = -1;

            return new WriteMemoryResponse {
                Offset = offset,
                BytesWritten = rawData.Length
            };
        }

        protected override EvaluateResponse HandleEvaluateRequest(EvaluateArguments arguments) {
            var split = arguments.Expression.Split(new string[] { ":/?" }, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length != 3) {
                return new EvaluateResponse { Result = null };
            }

            var name = split[0];
            int.TryParse(split[1], out var line);
            var filePath = split[2];

            try {
                if (name.StartsWith('$')) {
                    var register = debugInfo.Frames.First().Scopes.SelectMany(x => x.Variables).First(x => x.Name.Equals(name[1..], StringComparison.OrdinalIgnoreCase));
                    return new EvaluateResponse {
                        Result = $"Reg ${register?.Name} = {register?.Value}"
                    };
                }

                var constantValue = debugInfo.ConstantValues.First(x => x.FilePath.ComparePath(filePath) && x.Line <= line + 1 && x.Name == name);
                return new EvaluateResponse {
                    Result = constantValue.IsRegisterAlias
                        ? $"{constantValue.Name} (${constantValue.RegisterName}) = {constantValue.Value}"
                        : $"{constantValue.Name} = {constantValue.Value}"
                };

            } catch {
                return new EvaluateResponse { Result = name };
            }
        }

        private void SessionController_TerminatedEvent() {
            Protocol.SendEvent(new TerminatedEvent());
        }

        private void SessionController_OutputEvent(OutputType outputType, string message) {
            SendOutput(outputType, message);
        }

        private void SessionController_InvalidatedEvent(DebugInfo debugInfo) {
            this.debugInfo = debugInfo;
            Protocol.SendEvent(new InvalidatedEvent());
        }

        private void SessionController_ExitedEvent(int obj) {
            Protocol.SendEvent(new ExitedEvent());
        }

        private void SessionController_PausedEvent(PauseReasonType pauseReason, DebugInfo debugInfo) {
            this.debugInfo = debugInfo;

            pauseId++;
            Protocol.SendEvent(new StoppedEvent {
                Reason = (StoppedEvent.ReasonValue)(int)pauseReason,
                ThreadId = 0,
                HitBreakpointIds = debugInfo.HitBreakpointId.HasValue ? new List<int> { debugInfo.HitBreakpointId.Value } : null,
            });

            Protocol.SendEvent(new MemoryEvent { MemoryReference = RamMemoryReference });
        }

        private void SendOutput(OutputType outputType, string message) {
            Protocol.SendEvent(new OutputEvent { Category = (OutputEvent.CategoryValue)(int)outputType, Output = message });
        }
    }
}
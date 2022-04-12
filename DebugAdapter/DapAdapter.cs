using DebugAdapter.Configuration;
using DebugAdapter.Mappers;
using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol;
using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Messages;
using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Utilities;
using Runner.Debugger;
using Runner.Debugger.DebugData;
using Runner.Debugger.Enums;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DebugAdapter {
    public class DapAdapter : DebugAdapterBase {

        private readonly DapAdapterConfiguration configuration;
        private readonly DebugSessionController sessionController;
        private readonly Source source;

        private string errorMessage = null;
        private DebugInfo debugInfo;

        public DapAdapter(DapAdapterConfiguration configuration, DebugSessionController sessionController, Stream stdIn, Stream stdOut) {
            this.configuration = configuration;
            this.sessionController = sessionController;
            source = new Source { Path = configuration.SourceFilePath };

            SubscribeToEvents();
            InitializeProtocolClient(stdIn, stdOut);
        }

        private void SubscribeToEvents() {
            sessionController.InitializedEvent += SessionController_InitializedEvent;
            sessionController.OutputEvent += SessionController_OutputEvent;
            sessionController.PausedEvent += SessionController_PausedEvent;
            sessionController.InvalidatedEvent += SessionController_InvalidatedEvent;
            sessionController.ExitedEvent += SessionController_ExitedEvent;
            sessionController.TerminatedEvent += SessionController_TerminatedEvent;
        }

        internal void Run() {
            Protocol.Run();
        }

        internal void RunAndExitWithErrorMessage(string errorMessage) {
            this.errorMessage = errorMessage;
            Run();
        }

        protected override InitializeResponse HandleInitializeRequest(InitializeArguments arguments) {

            if (errorMessage != null) {
                throw new ProtocolException(errorMessage);
            }

            return new InitializeResponse {
                SupportsConfigurationDoneRequest = true,
                SupportsEvaluateForHovers = true,
                SupportsBreakpointLocationsRequest = true,
            };
        }

        protected override SetBreakpointsResponse HandleSetBreakpointsRequest(SetBreakpointsArguments arguments) {
            var unveryfied = arguments.Breakpoints
                .Select(x => x.ToUnverifiedBreakpoint());

            var verified = sessionController
                .GetPossibleBreakpointLocations()
                .Select(x => x.ToVerifiedBreakpoint(source));

            return new SetBreakpointsResponse {
                Breakpoints = verified.Concat(unveryfied.Except(verified)).ToList()
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
            // startujemy machine
            return new ConfigurationDoneResponse();
        }

        protected override AttachResponse HandleAttachRequest(AttachArguments arguments) {
            return new AttachResponse();
        }

        protected override LaunchResponse HandleLaunchRequest(LaunchArguments arguments) {
            string sourceFilePath = arguments.ConfigurationProperties.GetValueAsString("sourceFilePath");

            if (string.IsNullOrEmpty(sourceFilePath)) {
                throw new ProtocolException("Launch failed because launch configuration did not specify 'sourceFilePath'.");
            }

            if (!File.Exists(sourceFilePath)) {
                throw new ProtocolException($"Could not find the source file at: \"{sourceFilePath}\"");
            }

            sessionController.StartDebugging();

            /*try {
                var compiled = Assembler.Compiler.CompileFromFile(sourceFilePath, out var symbols);
                debugConfig = new() {
                    DebugSymbols = symbols,
                    StopAtEntry = arguments.ConfigurationProperties.GetValueAsBool("stopOnEntry") ?? false
                };
            } catch (Exception ex) {
                
            }*/



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
                        Name = "kpc"
                    }
                }
            };
        }

        protected override StackTraceResponse HandleStackTraceRequest(StackTraceArguments arguments) {
            return new StackTraceResponse {
                StackFrames = debugInfo.Frames
                    .Select(x => x.ToStackFrame(source, arguments))
                    .ToList()
            };
        }

        protected override ScopesResponse HandleScopesRequest(ScopesArguments arguments) {
            return new ScopesResponse {
                Scopes = debugInfo.Frames
                    .First().Scopes
                    .Select(s => s.ToScope(source))
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

        protected override EvaluateResponse HandleEvaluateRequest(EvaluateArguments arguments) {
            return new EvaluateResponse {
                Result = $"Jan paweł drugi {arguments.FrameId}",
            };
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

            Protocol.SendEvent(new StoppedEvent {
                Reason = (StoppedEvent.ReasonValue)(int)pauseReason,
                ThreadId = 0,
                HitBreakpointIds = new List<int> { debugInfo.HitBreakpointId },
            });
        }

        private void SessionController_InitializedEvent() {
            Protocol.SendEvent(new InitializedEvent());
        }

        private void SendOutput(OutputType outputType, string message) {
            Protocol.SendEvent(new OutputEvent { Category = (OutputEvent.CategoryValue)(int)outputType, Output = message });
        }
    }
}
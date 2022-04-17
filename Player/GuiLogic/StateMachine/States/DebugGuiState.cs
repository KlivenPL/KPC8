using Player._Configuration;
using Player._Infrastructure.Controls;
using Player._Infrastructure.Events;
using Player.Debugger;
using Player.Events;
using Player.Loaders;
using Player.MainForm;

namespace Player.GuiLogic.StateMachine.States {
    internal class DebugGuiState : CommonGuiState, IEventListener<DapAdapterStatusChangedEvent> {
        private readonly ProgramLoader programLoader;

        private DapAdapterAttachInitializer debugInitializer;

        public DebugGuiState(GuiStateManager guiStateManager, KPC8Player.Controller controller, ProgramLoader programLoader) : base(guiStateManager, controller) {
            this.programLoader = programLoader;
        }

        public override void OnEnter() {
            this.ListenToEvent<DapAdapterStatusChangedEvent>();
            Controller.mnuStopBtn.OnUI(x => x.Enabled = true);
            StartDebuggingServer();
        }

        public override void OnExit() {
            this.StopListenToEvent<DapAdapterStatusChangedEvent>();
            Controller.mnuStopBtn.OnUI(x => x.Enabled = false);
            debugInitializer = null;
            Controller.StatusTitle = null;
        }

        public override void Stop() {
            debugInitializer.Stop();
            SetState<StopGuiState>();
        }

        public void OnEvent(DapAdapterStatusChangedEvent @event) {
            switch (@event.Status) {
                case DapAdapterStatus.None:
                    Controller.StatusTitle = null;
                    Stop();
                    break;
                case DapAdapterStatus.AwaitingConnection:
                    Controller.StatusTitle = "Awaiting for external debugger";
                    break;
                case DapAdapterStatus.Connected:
                    Controller.StatusTitle = "External debugger attached";
                    break;
                default:
                    break;
            }
        }

        private void StartDebuggingServer() {
            if (programLoader.TryGetCompiledProgramWithDebugSymbols(out var sourceFilePath, out var program, out var debugSymbols, out var compileErrors)) {
                new Thread(() => {
                    debugInitializer = new DapAdapterAttachInitializer();

                    var parameters = new DebugAttachParameters {
                        CompiledProgram = program,
                        DebugSymbols = debugSymbols,
                        ServerPort = 32137, //todo do poprawy
                        KPC8ConfigurationDto = new _Configuration.Dtos.KPC8ConfigurationDto(),
                        SourceFilePath = sourceFilePath
                    };

                    debugInitializer.InitializeAttachServer(parameters);

                }).Start();

                Controller.UnfreezeForm();
            } else {
                if (!string.IsNullOrWhiteSpace(compileErrors)) {
                    MessageBox.Show(compileErrors, "Compile error");
                }
                Controller.UnfreezeForm();
                SetState<StopGuiState>();
            }
        }
    }
}

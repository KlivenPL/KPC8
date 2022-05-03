using Player._Configuration;
using Player._Configuration.Dtos;
using Player._Infrastructure.Controls;
using Player._Infrastructure.Events;
using Player.Debugger;
using Player.Events;
using Player.Loaders;
using Player.MainForm;
using Runner.GraphicsRending;

namespace Player.GuiLogic.StateMachine.States {
    internal class DebugGuiState : CommonGuiState, IEventListener<DapAdapterStatusChangedEvent> {
        private readonly ProgramLoader programLoader;
        private readonly KPC8ConfigurationLoader configurationLoader;

        private DapAdapterAttachInitializer debugInitializer;
        private RendererController rendererController;

        public DebugGuiState(GuiStateManager guiStateManager, KPC8Player.Controller controller, ProgramLoader programLoader, KPC8ConfigurationLoader configurationLoader) : base(guiStateManager, controller) {
            this.programLoader = programLoader;
            this.configurationLoader = configurationLoader;
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
            rendererController = null;
            Controller.StatusTitle = null;
        }

        public override void Stop() {
            debugInitializer.Stop();
        }

        public void OnEvent(DapAdapterStatusChangedEvent @event) {
            switch (@event.Status) {
                case DapAdapterStatus.None:
                    Controller.StatusTitle = null;
                    CleanUpAndExit();
                    break;
                case DapAdapterStatus.AwaitingConnection:
                    Controller.StatusTitle = "Awaiting for external debugger";
                    break;
                case DapAdapterStatus.Connected:
                    Controller.StatusTitle = "External debugger attached";
                    StartRendering();
                    break;
                default:
                    break;
            }
        }

        private void StartDebuggingServer() {
            if (programLoader.TryGetCompiledProgramWithDebugSymbols(out var sourceFilePath, out var program, out var debugSymbols, out var compileErrors)) {

                var kPC8ConfigurationDto = new KPC8ConfigurationDto();

                if (configurationLoader.TryGetConfiguration(out var configurationDto, out var configValidationErrors)) {
                    kPC8ConfigurationDto = configurationDto;
                } else if (!string.IsNullOrWhiteSpace(configValidationErrors)) {
                    MessageBox.Show(configValidationErrors, "KPC Configuration validation errors");
                    Controller.UnfreezeForm();
                    SetState<StopGuiState>();
                    return;
                }

                new Thread(() => {
                    debugInitializer = new DapAdapterAttachInitializer();

                    var parameters = new DebugAttachParameters {
                        CompiledProgram = program,
                        DebugSymbols = debugSymbols,
                        ServerPort = 32137, //todo do poprawy
                        KPC8ConfigurationDto = kPC8ConfigurationDto,
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

        private void StartRendering() {
            rendererController = debugInitializer.AttachRenderer();
            rendererController.CanvasWriteEvent += Controller.SetRenderCanvasBitmap;
            rendererController.StartRendering(120);
        }

        private void CleanUpAndExit() {
            if (rendererController != null) {
                rendererController.CanvasWriteEvent -= Controller.SetRenderCanvasBitmap;
            }

            SetState<StopGuiState>();
        }
    }
}

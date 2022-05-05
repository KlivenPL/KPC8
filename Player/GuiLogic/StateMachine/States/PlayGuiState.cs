using Player._Configuration.Dtos;
using Player._Infrastructure.Controls;
using Player.Loaders;
using Player.MainForm;
using Player.Properties;
using Runner.Configuration;
using Runner.GraphicsRending;
using Runner.Player;

namespace Player.GuiLogic.StateMachine.States {
    internal class PlayGuiState : CommonGuiState {
        private const string PlayingTitle = "Playing";
        private const string PausedTitle = "Paused";

        private readonly ProgramLoader programLoader;
        private readonly KPC8ConfigurationLoader configurationLoader;

        private PlaySessionController playSessionController;
        private RendererController rendererController;
        private bool paused = false;

        public PlayGuiState(GuiStateManager guiStateManager, KPC8Player.Controller controller, ProgramLoader programLoader, KPC8ConfigurationLoader configurationLoader) : base(guiStateManager, controller) {
            this.programLoader = programLoader;
            this.configurationLoader = configurationLoader;
        }

        public override void OnEnter() {
            Controller.mnuStopBtn.OnUI(x => x.Enabled = true);
            Controller.mnuPauseBtn.OnUI(x => x.Enabled = true);
            Application.ApplicationExit += OnApplicationExit;
            StartPlaying();
        }

        public override void OnExit() {
            Controller.mnuStopBtn.OnUI(x => x.Enabled = false);
            Controller.mnuPauseBtn.OnUI(x => x.Enabled = false);
            paused = false;
            playSessionController = null;
            rendererController = null;
            Controller.StatusTitle = null;
        }

        public override void Stop() {
            playSessionController?.Terminate();
        }

        public override void Pause() {
            if (paused) {
                Controller.mnuPauseBtn.Image = (Image)Resources.ResourceManager.GetObject("pause");
                Controller.mnuPauseBtn.ToolTipText = "Pause";
                Controller.StatusTitle = PlayingTitle;
                playSessionController.Continue();
            } else {
                Controller.FreezeFrom();
                playSessionController.Pause();
            }

            paused = !paused;
        }

        private void OnApplicationExit(object sender, EventArgs e) {
            playSessionController?.Terminate();
        }

        private void StartPlaying() {
            if (programLoader.TryGetCompiledProgram(out var program, out var compileErrors)) {

                var kPC8ConfigurationDto = new KPC8ConfigurationDto();

                if (configurationLoader.TryGetConfiguration(out var configurationDto, out var configValidationErrors)) {
                    kPC8ConfigurationDto = configurationDto;
                } else if (!string.IsNullOrWhiteSpace(configValidationErrors)) {
                    MessageBox.Show(configValidationErrors, "KPC Configuration validation errors");
                    Controller.UnfreezeForm();
                    SetState<StopGuiState>();
                    return;
                }

                var kpcConfiguration = new KPC8Configuration {
                    ClockMode = Components.Clocks.ClockMode.Automatic,
                    ClockPeriodInTicks = kPC8ConfigurationDto.ClockPeriodInTicks ?? 5,
                    ExternalModules = kPC8ConfigurationDto.ExternalModules,
                    InitialRamData = kPC8ConfigurationDto.InitialRamData,
                    RomData = program
                };

                playSessionController = PlaySessionController.Factory.Create(kpcConfiguration);
                playSessionController.TerminatedEvent += OnPlaySessionExit;
                playSessionController.ExitedEvent += _ => OnPlaySessionExit();
                playSessionController.PausedEvent += OnPlaySessionPaused;
                playSessionController.StartPlaying();

                StartRendering();
                Controller.StatusTitle = PlayingTitle;

                Controller.UnfreezeForm();
            } else {
                if (!string.IsNullOrWhiteSpace(compileErrors)) {
                    MessageBox.Show(compileErrors, "Compile error");
                }
                Controller.UnfreezeForm();
                SetState<StopGuiState>();
            }
        }

        private void OnPlaySessionPaused() {
            Controller.StatusTitle = PausedTitle;
            Controller.mnuPauseBtn.ToolTipText = "Resume";
            Controller.mnuPauseBtn.Image = (Image)Resources.ResourceManager.GetObject("play");
            Controller.UnfreezeForm();
        }

        private void OnPlaySessionExit() {
            CleanUpAndExit();
        }

        private void StartRendering() {
            rendererController = playSessionController.AttachRenderer();
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

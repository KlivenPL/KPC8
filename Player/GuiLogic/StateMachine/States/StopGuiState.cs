using Player._Infrastructure.Controls;
using Player._Infrastructure.Events;
using Player.Contexts;
using Player.Events;
using Player.MainForm;

namespace Player.GuiLogic.StateMachine.States {
    internal class StopGuiState : CommonGuiState, IEventListener<LoadedProgramChangedEvent> {
        private readonly ProgramContext programContext;
        public StopGuiState(GuiStateManager guiStateManager, KPC8Player.Controller controller, ProgramContext programContext) : base(guiStateManager, controller) {
            this.programContext = programContext;
        }

        public override void OnEnter() {
            this.ListenToEvent<LoadedProgramChangedEvent>();
            Controller.mnuPlayBtn.OnUI(x => x.Enabled = true);
            Controller.mnuDbgBtn.OnUI(x => x.Enabled = true);

            Controller.mnuFileLoadRomBtn.OnUI(x => x.Enabled = true);
            Controller.mnuFileLoadSourceBtn.OnUI(x => x.Enabled = true);

            Controller.ResetRenderCanvas();
        }

        public override void OnExit() {
            this.StopListenToEvent<LoadedProgramChangedEvent>();
            Controller.mnuPlayBtn.OnUI(x => x.Enabled = false);
            Controller.mnuDbgBtn.OnUI(x => x.Enabled = false);

            Controller.mnuFileLoadRomBtn.OnUI(x => x.Enabled = false);
            Controller.mnuFileLoadSourceBtn.OnUI(x => x.Enabled = false);
        }

        public override void Play() {
            Controller.FreezeFrom();
            SetState<PlayGuiState>();
        }

        public override void Debug() {
            Controller.FreezeFrom();
            SetState<DebugGuiState>();
        }

        public override void StepInto() {

        }

        public void OnEvent(LoadedProgramChangedEvent @event) {
            if (@event.RomFile != null) {
                Controller.LoadedFileName = @event.RomFile.Name;
            } else if (@event.SourceFile != null) {
                Controller.LoadedFileName = @event.SourceFile.Name;
            } else {
                Controller.LoadedFileName = null;
            }
            Controller.mnuFileLoadRomBtn.OnUI(x => x.Enabled = true);
            Controller.mnuFileLoadSourceBtn.OnUI(x => x.Enabled = true);
        }
    }
}

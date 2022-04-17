using Player._Infrastructure.Controls;
using Player.MainForm;

namespace Player.GuiLogic.StateMachine.States {
    internal class StopGuiState : GuiStateBase {
        public StopGuiState(GuiStateManager guiStateManager, KPC8Player.Controller controller) : base(guiStateManager, controller) {

        }

        public override void OnEnter() {
            Controller.mnuPlayBtn.OnUiThread(x => x.Enabled = true);
            Controller.mnuDbgBtn.OnUiThread(x => x.Enabled = true);
            Controller.mnuStepIntoBtn.OnUiThread(x => x.Enabled = true);
        }

        public override void OnExit() {
            Controller.mnuPlayBtn.OnUiThread(x => x.Enabled = false);
            Controller.mnuDbgBtn.OnUiThread(x => x.Enabled = false);
            Controller.mnuStepIntoBtn.OnUiThread(x => x.Enabled = false);
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
    }
}

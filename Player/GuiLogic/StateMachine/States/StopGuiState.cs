using Player.MainForm;

namespace Player.GuiLogic.StateMachine.States {
    internal class StopGuiState : GuiStateBase {
        public StopGuiState(GuiStateManager guiStateManager, KPC8Player.Controller controller) : base(guiStateManager, controller) { }

        public override GuiStateType GuiStateType => GuiStateType.Stop;

        public override void OnEnter() {
            Controller.mnuPlayBtn.Enabled = true;
            Controller.mnuDbgBtn.Enabled = true;
            Controller.mnuStepIntoBtn.Enabled = true;
        }

        public override void OnExit() {
            Controller.mnuPlayBtn.Enabled = false;
            Controller.mnuDbgBtn.Enabled = false;
            Controller.mnuStepIntoBtn.Enabled = false;
        }

        public override void Play() {
            SetState<PlayGuiState>();
        }

        public override void Debug() {

        }

        public override void StepInto() {

        }
    }
}

using Player.MainForm;

namespace Player.GuiLogic.StateMachine.States {
    internal abstract class CommonGuiState : GuiStateBase {
        public CommonGuiState(GuiStateManager guiStateManager, KPC8Player.Controller controller) : base(guiStateManager, controller) {
        }
    }
}

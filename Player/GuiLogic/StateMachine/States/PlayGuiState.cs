﻿using Player.MainForm;

namespace Player.GuiLogic.StateMachine.States {
    internal class PlayGuiState : CommonGuiState {
        public PlayGuiState(GuiStateManager guiStateManager, KPC8Player.Controller controller) : base(guiStateManager, controller) { }

        public override void OnEnter() {
            Controller.mnuPauseBtn.Enabled = true;
            Controller.mnuStopBtn.Enabled = true;
        }

        public override void OnExit() {
            Controller.mnuPauseBtn.Enabled = false;
            Controller.mnuStopBtn.Enabled = false;
        }

        public override void Stop() {
            SetState<StopGuiState>();
        }
    }
}
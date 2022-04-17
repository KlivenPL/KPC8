using Player._Infrastructure.States;
using Player.MainForm;

namespace Player.GuiLogic.StateMachine {
    internal abstract class GuiStateBase : GuiStateManager.GuiStateSetter, IGuiState {
        protected KPC8Player.Controller Controller;

        protected GuiStateBase(GuiStateManager guiStateManager, KPC8Player.Controller controller) : base(guiStateManager) {
            Controller = controller;
        }

        public virtual void OnEnter() {

        }

        public virtual void OnExit() {

        }

        public virtual void BreakpointHit() {
            throw new InvalidOperationException();
        }

        public virtual void Debug() {
            throw new InvalidOperationException();
        }

        public virtual void Pause() {
            throw new InvalidOperationException();
        }

        public virtual void Play() {
            throw new InvalidOperationException();
        }

        public virtual void StepInto() {
            throw new InvalidOperationException();
        }

        public virtual void StepOver() {
            throw new InvalidOperationException();
        }

        public virtual void Stop() {
            throw new InvalidOperationException();
        }
    }
}

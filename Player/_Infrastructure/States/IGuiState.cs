namespace Player._Infrastructure.States {
    internal interface IGuiState {
        void OnEnter();
        void OnExit();

        #region PlayMode

        void Play();
        void Pause();

        #endregion

        #region DebugMode

        void Debug();
        void StepInto();
        void StepOver();
        void BreakpointHit();

        #endregion

        void Stop();
    }
}

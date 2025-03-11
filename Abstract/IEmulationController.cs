namespace Abstract {
    public interface IEmulationController {
        void InitializePlay();
        void InitializeDebug();
        void ExecuteSingleInstruction();
        void Execute();
        void Terminate();
    }
}

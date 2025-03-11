using Abstract;
using LightweightEmulator.Kpc;

namespace LightweightEmulator.Pipelines {
    public class LwEmulationController : IEmulationController {
        private readonly LwKpcBuild _kpcBuild;
        private readonly LwInstructionExecutor _lwExecutor;

        public LwEmulationController(LwKpcBuild kpcBuild) {
            _kpcBuild = kpcBuild;
            _lwExecutor = new(kpcBuild);
        }

        public void Execute() => ExecuteSingleInstruction();

        public void ExecuteSingleInstruction() {
            var lo = _kpcBuild.Rom.ReadByte(_kpcBuild.Pc.WordValue);
            var hi = _kpcBuild.Rom.ReadByte((ushort)(_kpcBuild.Pc.WordValue + 1));

            var instruction = new LwInstruction(lo, hi);
            _lwExecutor.Execute(instruction);
        }

        public void InitializeDebug() => Initialize();

        public void InitializePlay() => Initialize();

        private void Initialize() {

        }

        public void Terminate() {

        }
    }
}

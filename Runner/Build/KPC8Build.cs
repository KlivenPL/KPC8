using Components.Clocks;
using KPC8.ControlSignals;
using Simulation.Loops;
using System.Collections.Generic;

namespace Runner.Build {
    internal class KPC8Build {
        private readonly Clock mainClock;
        private readonly CsPanel csPanel;
        private readonly ModulePanel modulePanel;
        private readonly SimulationLoop mainSimulationLoop;
        private readonly IReadOnlyList<SimulationLoop> externalSimulationLoops;

        public Clock MainClock => mainClock;
        public CsPanel CsPanel => csPanel;
        public ModulePanel ModulePanel => modulePanel;
        public SimulationLoop MainSimulationLoop => mainSimulationLoop;
        public IReadOnlyList<SimulationLoop> ExternalSimulationLoops => externalSimulationLoops;

        public KPC8Build(
            Clock mainClock,
            CsPanel csPanel,
            ModulePanel modulePanel,
            SimulationLoop mainSimulationLoop,
            IReadOnlyList<SimulationLoop> externalSimulationLoops) {

            this.csPanel = csPanel;
            this.modulePanel = modulePanel;
            this.mainSimulationLoop = mainSimulationLoop;
            this.externalSimulationLoops = externalSimulationLoops;
            this.mainClock = mainClock;

            Initialize();
        }

        private void Initialize() {
            for (int i = 0; i < 4; i++) {
                mainSimulationLoop.Loop();
                foreach (var externalSimulationLoop in externalSimulationLoops) {
                    externalSimulationLoop.Loop();
                }
            }

            mainClock.Disabled = false;
        }
    }
}

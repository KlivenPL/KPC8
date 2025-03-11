using _Infrastructure.Simulation.Loops;
using Abstract;
using Abstract.Components;
using Components.Clocks;
using KPC8.ControlSignals;
using System.Collections.Generic;

namespace Runner.Build {
    internal class KPC8Build : IKpcBuild {
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

        public IRegister16[] ProgrammerRegisters => ModulePanel.Registers.Regs;

        public IMemory Rom => ModulePanel.Memory.Rom;

        public IMemory Ram => ModulePanel.Memory.Ram;

        public IRegister16 Pc => ModulePanel.Memory.Pc;

        public IRegister16 Mar => ModulePanel.Memory.Mar;

        public IRegister4 Flags => ModulePanel.Alu.Flags;

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
            for (int i = 0; i < 10; i++) {
                mainSimulationLoop.Loop();
                foreach (var externalSimulationLoop in externalSimulationLoops) {
                    externalSimulationLoop.Loop();
                }
            }

            mainClock.Disabled = false;
        }
    }
}

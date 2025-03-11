using _Infrastructure.Simulation.Loops;
using Abstract;
using Runner.Build;
using System.Collections.Generic;

namespace Runner.EmulationControl {
    internal class KPC8EmulationController : IEmulationController {
        private readonly KPC8Build kpc;
        private readonly List<SimulationLoopRunner> externalSlRunners;

        public KPC8EmulationController(KPC8Build kpc) {
            this.kpc = kpc;
            externalSlRunners = new();
        }

        public void ExecuteSingleInstruction() {
            // Execute currently loaded instruction
            while (!kpc.CsPanel.Ctrl.Ic_clr) {
                MakeTickAndWait();
            }

            // Load next instruction
            MakeTickAndWait();
            MakeTickAndWait();
            MakeTickAndWait();
            MakeTickAndWait();
        }

        public void Execute() {
            for (int i = 0; i < 16; i++) {
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
                kpc.MainSimulationLoop.Loop();
            }
        }

        public void InitializePlay() {
            foreach (var externalModuleSl in kpc.ExternalSimulationLoops) {
                externalSlRunners.Add(SimulationLoopRunner.RunInNewThread(externalModuleSl));
            }
        }

        public void InitializeDebug() {
            foreach (var externalModuleSl in kpc.ExternalSimulationLoops) {
                externalSlRunners.Add(SimulationLoopRunner.RunInNewThread(externalModuleSl));
            }

            // Load the first instruction
            MakeTickAndWait();
            MakeTickAndWait();
            MakeTickAndWait();
        }

        public void Terminate() {
            foreach (var externalSlRunner in externalSlRunners) {
                externalSlRunner.Kill();
            }

            externalSlRunners.Clear();
        }

        private void MakeTickAndWait() {
            kpc.MainClock.MakeTick();

            while (kpc.MainClock.IsManualTickInProgress) {
                kpc.MainSimulationLoop.Loop();
            }
        }
    }
}

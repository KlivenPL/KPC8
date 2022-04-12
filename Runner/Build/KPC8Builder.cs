using Components.Clocks;
using Components.Signals;
using KPC8.ControlSignals;
using Runner.Configuration;
using Simulation.Loops;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Runner.Build {
    internal class KPC8Builder {
        private readonly KPC8Configuration configuration;

        internal KPC8Builder(KPC8Configuration configuration) {
            this.configuration = configuration ?? new KPC8Configuration();
        }

        internal KPC8Build Build() {
            var clk = Signal.Factory.Create("Clock");
            var clkBar = Signal.Factory.Create("ClockBar");

            using var loopBuilder = SimulationLoopBuilder.CreateAsCurrent().SetName("Main");

            var clock = new Clock(clk, clkBar, configuration.ClockMode, configuration.ClockPeriodInTicks);
            clock.Disabled = true;

            var cpuBuilder = new CpuBuilder(clock)
                .WithControlModule(null, true)
                .WithMemoryModule(configuration.RomData, configuration.InitialRamData)
                .WithRegistersModule()
                .WithAluModule();

            List<Func<SimulationLoop>> simulationLoopGetters = new List<Func<SimulationLoop>>();
            foreach (var externalModuleConfig in configuration.ExternalModules) {
                externalModuleConfig.Configure(cpuBuilder, out var getSimulationLoop);
                simulationLoopGetters.Add(getSimulationLoop);
            }

            var csPanel = cpuBuilder.BuildWithModulesAccess(out var modulePanel);
            var mainSimulationLoop = loopBuilder.Build();
            var externalSimulationLoops = simulationLoopGetters.Select(sg => sg()).ToList();

            return new KPC8Build(clock, csPanel, modulePanel, mainSimulationLoop, externalSimulationLoops);
        }
    }
}

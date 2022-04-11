using KPC8.ControlSignals;
using Simulation.Loops;
using System;

namespace Runner.Configuration.ExternalModules {
    internal interface IExternalModuleConfiguration {
        void Configure(CpuBuilder cpuBuilder, out Func<SimulationLoop> getSimulationLoop);
    }
}

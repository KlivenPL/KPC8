using KPC8.ControlSignals;
using _Infrastructure.Simulation.Loops;
using System;

namespace Runner.Configuration.ExternalModules {
    public interface IExternalModuleConfiguration {
        void Configure(CpuBuilder cpuBuilder, out Func<SimulationLoop> getSimulationLoop);
    }
}

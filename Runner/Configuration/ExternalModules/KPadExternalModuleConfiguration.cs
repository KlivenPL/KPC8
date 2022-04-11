using KPC8.ControlSignals;
using KPC8.ExternalModules;
using Simulation.Loops;
using System;

namespace Runner.Configuration.ExternalModules {
    public class KPadExternalModuleConfiguration : IExternalModuleConfiguration {
        public string Name { get; set; }
        public ushort Address { get; set; }

        void IExternalModuleConfiguration.Configure(CpuBuilder cpuBuilder, out Func<SimulationLoop> getSimulationLoop) {
            cpuBuilder.AddKPad(Name, Address, out _, out getSimulationLoop);
        }
    }
}

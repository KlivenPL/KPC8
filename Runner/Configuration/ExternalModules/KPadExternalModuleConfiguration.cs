using KPC8.ControlSignals;
using KPC8.ExternalModules;
using _Infrastructure.Simulation.Loops;
using System;

namespace Runner.Configuration.ExternalModules {
    public class KPadExternalModuleConfiguration : IExternalModuleConfiguration {
        public string PadName { get; set; }
        public ushort PadAddress { get; set; }

        void IExternalModuleConfiguration.Configure(CpuBuilder cpuBuilder, out Func<SimulationLoop> getSimulationLoop) {
            cpuBuilder.AddKPad(PadName, PadAddress, out _, out getSimulationLoop);
        }
    }
}

using KPC8.ControlSignals;
using KPC8.ExternalModules;
using _Infrastructure.Simulation.Loops;
using System;

namespace Runner.Configuration.ExternalModules {
    public class KTimerExternalModuleConfiguration : IExternalModuleConfiguration {
        public string TimerName { get; set; }
        public ushort TimerAddress { get; set; }

        void IExternalModuleConfiguration.Configure(CpuBuilder cpuBuilder, out Func<SimulationLoop> getSimulationLoop) {
            cpuBuilder.AddKTimer(TimerName, TimerAddress, out _, out getSimulationLoop);
        }
    }
}

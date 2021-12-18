using Autofac;
using Simulation.Loops;

namespace Simulation._Configuration {
    public class SimulationModule : Module {
        protected override void Load(ContainerBuilder builder) {
            builder.RegisterType<SimulationLoop>()
                .InstancePerLifetimeScope();
        }
    }
}

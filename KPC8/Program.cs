using Autofac;
using Autofac.Features.AttributeFilters;
using Components._Configuration;
using Simulation._Configuration;

namespace KPC8 {
    class Program {
        static void Main(string[] args) {
            using var scope = CompositionRoot().BeginLifetimeScope();
            scope.Resolve<Application>().Run();
        }

        private static IContainer CompositionRoot() {
            var builder = new ContainerBuilder();
            builder.RegisterType<Application>().WithAttributeFiltering();
            builder.RegisterModule<ComponentsModule>();
            builder.RegisterModule<SimulationModule>();
            return builder.Build();
        }
    }
}

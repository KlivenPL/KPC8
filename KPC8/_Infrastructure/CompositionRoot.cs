using Autofac;
using Autofac.Features.AttributeFilters;
using KPC8._Infrastructure.Modules;
using Simulation._Configuration;

namespace KPC8._Infrastructure {
    public static class CompositionRoot {
        public static ILifetimeScope BeginLifetimeScope() {
            return CreateCompositionRoot().BeginLifetimeScope();
        }

        private static IContainer CreateCompositionRoot() {
            var builder = new ContainerBuilder();
            builder.RegisterType<Application>().WithAttributeFiltering();

            builder.RegisterModule<ClocksModule>();
            builder.RegisterModule<SimulationModule>();
            builder.RegisterModule<SignalsModule>();

            return builder.Build();
        }
    }
}

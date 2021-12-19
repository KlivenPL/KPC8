using Autofac;
using Simulation.Updates;
using System.Reflection;

namespace KPC8._Infrastructure.Modules {
    class UpdatesModule : Autofac.Module {

        private Assembly[] assemblies;

        protected override void Load(ContainerBuilder builder) {
            assemblies = new[]{
                Assembly.Load(nameof(Components)),
                ThisAssembly
            };

            RegisterUpdates(builder);
        }

        private void RegisterUpdates(ContainerBuilder builder) {
            builder.RegisterAssemblyTypes(assemblies)
                .AssignableTo<IUpdate>()
                .As<IUpdate>()
                .IfNotRegistered(typeof(IUpdate))
                .InstancePerLifetimeScope();
        }
    }
}

using Autofac;
using Components.Signals;

namespace KPC8._Infrastructure.Modules {
    class SignalsModule : Module {

        protected override void Load(ContainerBuilder builder) {
            RegisterSignalPorts(builder);
        }

        private void RegisterSignalPorts(ContainerBuilder builder) {
            builder.RegisterType<SignalPort>()
                .InstancePerDependency();
        }
    }
}

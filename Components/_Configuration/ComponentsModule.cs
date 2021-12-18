using Autofac;
using Components.Clocks;
using Simulation.Updates;
using System;
using System.Linq;
using System.Reflection;

namespace Components._Configuration {
    public class ComponentsModule : Autofac.Module {
        protected override void Load(ContainerBuilder builder) {
            RegisterClocks(builder);
            RegisterUpdates(builder);
        }

        private void RegisterUpdates(ContainerBuilder builder) {
            builder.RegisterAssemblyTypes(ThisAssembly)
                .AssignableTo<IUpdate>()
                .As<IUpdate>()
                .IfNotRegistered(typeof(IUpdate))
                .InstancePerLifetimeScope();
        }

        private void RegisterClocks(ContainerBuilder builder) {
            var clockTypes = Enum.GetValues<ClockType>();

            foreach (var clockType in clockTypes) {
                var parameters = GetCustomAttribute<ClockParametersAttribute>(clockType);

                builder
                    .RegisterType<Clock>()
                    .Keyed<Clock>(clockType)
                    .WithParameters(new[] {
                        new TypedParameter(typeof(ClockMode), parameters.ClockMode),
                        new TypedParameter(typeof(double), parameters.Frequency),
                        new TypedParameter(typeof(ClockType), clockType),
                    })
                    .AsSelf()
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();
            }
        }

        private static T GetCustomAttribute<T>(Enum enumeration) where T : Attribute {
            FieldInfo fi = enumeration.GetType().GetField(enumeration.ToString());

            if (fi.GetCustomAttributes(typeof(T), false) is T[] attributes && attributes.Any()) {
                return attributes.First();
            }

            return null;
        }
    }
}

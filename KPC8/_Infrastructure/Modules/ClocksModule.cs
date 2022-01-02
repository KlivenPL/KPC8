using _Infrastructure.Enums;
using Autofac;
using Autofac.Core;
using Components.Clocks;
using Components.Signals;
using KPC8.Clocks;
using System;
using System.Reflection;

namespace KPC8._Infrastructure.Modules {
    class ClocksModule : Autofac.Module {

        protected override void Load(ContainerBuilder builder) {
            RegisterClocks(builder);
        }

        private void RegisterClocks(ContainerBuilder builder) {
            var clockTypes = Enum.GetValues<ClockType>();

            foreach (var clockType in clockTypes) {
                var parameters = clockType.GetCustomAttribute<ClockParametersAttribute>();

                builder
                    .RegisterType<Clock>()
                    .Keyed<Clock>(clockType)
                    .WithParameters(new ConstantParameter[] {
                        new TypedParameter(typeof(ClockMode), parameters.ClockMode),
                        new TypedParameter(typeof(long), parameters.PeriodInTicks),
                        new NamedParameter("clkSignal", Signal.Factory.Create($"sig_{clockType}")),
                        new NamedParameter("clkBarSignal", Signal.Factory.Create($"sig_{clockType}_bar")),
                    })
                    .AsSelf()
                    .AsImplementedInterfaces()
                    .SingleInstance();
            }
        }
    }
}

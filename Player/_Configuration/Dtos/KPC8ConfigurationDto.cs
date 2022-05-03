using Player.Persistence;
using Runner.Configuration.ExternalModules;
using System.Collections;

namespace Player._Configuration.Dtos {
    internal class KPC8ConfigurationDto {
        public long? ClockPeriodInTicks { get; set; } = null;
        public BitArray[] InitialRamData { get; set; } = null;
        public List<IExternalModuleConfiguration> ExternalModules { get; set; } = new List<IExternalModuleConfiguration>();

        public static KPC8ConfigurationDto FromSave(KPC8ConfigurationSave save) {

            var kpads = save.KPads ?? Array.Empty<IExternalModuleConfiguration>();
            var kTimers = save.KTimers ?? Array.Empty<IExternalModuleConfiguration>();

            return new KPC8ConfigurationDto {
                ClockPeriodInTicks = save.ClockPeriodInTicks,
                ExternalModules = kpads
                    .Concat(kTimers)
                    .ToList(),
                InitialRamData = null,
            };
        }
    }
}

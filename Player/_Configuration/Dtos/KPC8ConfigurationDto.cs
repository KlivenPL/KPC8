using Components.Clocks;
using Runner.Configuration.ExternalModules;
using System.ComponentModel.DataAnnotations;

namespace Player._Configuration.Dtos {
    internal class KPC8ConfigurationDto {
        public ClockMode ClockMode { get; set; }

        [Range(3, 10000)]
        public long? ClockPeriodInTicks { get; set; }

        [MaxLength(ushort.MaxValue + 1)]
        public List<byte> InitialRamData { get; set; }

        public List<KPadExternalModuleConfiguration> KPads { get; set; }
    }
}

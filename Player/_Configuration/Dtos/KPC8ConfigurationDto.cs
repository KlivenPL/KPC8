using Runner.Configuration.ExternalModules;
using System.Collections;

namespace Player._Configuration.Dtos {
    internal class KPC8ConfigurationDto {
        public long? ClockPeriodInTicks { get; set; } = null;
        public BitArray[] InitialRamData { get; set; } = null;
        public List<IExternalModuleConfiguration> ExternalModules { get; set; } = new List<IExternalModuleConfiguration>();
    }
}

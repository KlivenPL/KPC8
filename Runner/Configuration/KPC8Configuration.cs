using Components.Clocks;
using Runner.Configuration.ExternalModules;
using System.Collections;
using System.Collections.Generic;

namespace Runner.Configuration {
    internal class KPC8Configuration {
        public ClockMode ClockMode { get; init; } = ClockMode.Automatic;
        public long ClockPeriodInTicks { get; init; } = 3;
        public BitArray[] RomData { get; init; } = null;
        public BitArray[] InitialRamData { get; init; } = null;
        public List<IExternalModuleConfiguration> ExternalModules { get; init; } = new List<IExternalModuleConfiguration>();
    }
}

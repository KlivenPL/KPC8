using Runner.Configuration.ExternalModules;
using System.ComponentModel.DataAnnotations;

namespace Player.Persistence {
    internal class KPC8ConfigurationSave {

        [Range(5, 10000)]
        public long? ClockPeriodInTicks { get; set; }

        public KPadExternalModuleConfiguration[] KPads { get; set; }

        public KTimerExternalModuleConfiguration[] KTimers { get; set; }
    }
}

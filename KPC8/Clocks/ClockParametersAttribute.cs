using Components.Clocks;
using System;

namespace KPC8.Clocks {
    public class ClockParametersAttribute : Attribute {
        public ClockParametersAttribute(long periodInTicks, ClockMode clockMode) {
            PeriodInTicks = periodInTicks;
            ClockMode = clockMode;
        }

        public long PeriodInTicks { get; }
        public ClockMode ClockMode { get; }
    }
}

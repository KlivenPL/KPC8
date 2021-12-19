using Components.Clocks;
using System;

namespace KPC8.Clocks {
    class ClockParametersAttribute : Attribute {
        public ClockParametersAttribute(double frequency, ClockMode clockMode) {
            Frequency = frequency;
            ClockMode = clockMode;
        }

        public double Frequency { get; }
        public ClockMode ClockMode { get; }
    }
}

using _Infrastructure.Enums;

namespace KPC8.Clocks {
    public static class ClockTypeExtensions {
        public static ClockParametersAttribute GetClockParameters(this ClockType clockType) {
            return clockType.GetCustomAttribute<ClockParametersAttribute>();
        }
    }
}

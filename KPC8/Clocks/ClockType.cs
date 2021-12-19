using Components.Clocks;

namespace KPC8.Clocks {
    public enum ClockType {
        [ClockParameters(1.0, ClockMode.Automatic)]
        MainClock,

        [ClockParameters(10, ClockMode.Manual)]
        TestManualClock,
    }
}

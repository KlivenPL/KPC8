using Components.Clocks;

namespace KPC8.Clocks {
    public enum ClockType {
        [ClockParameters(100, ClockMode.Automatic)]
        MainClock,

        [ClockParameters(10000, ClockMode.Manual)]
        TestManualClock,
    }
}

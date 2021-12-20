using Components.Clocks;

namespace KPC8.Clocks {
    public enum ClockType {
        [ClockParameters(4, ClockMode.Automatic)]
        MainClock,

        [ClockParameters(4, ClockMode.Manual)]
        TestManualClock,
    }
}

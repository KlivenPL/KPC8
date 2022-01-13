using Components.Clocks;

namespace KPC8.Clocks {
    public enum ClockType {
        [ClockParameters(3, ClockMode.Automatic)]
        MainClock,

        [ClockParameters(10000, ClockMode.Manual)]
        MainManualClock,

        [ClockParameters(3, ClockMode.Manual)]
        TestManualClock,
    }
}

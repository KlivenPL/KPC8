using Components.Clocks;

namespace KPC8.Clocks {
    public enum ClockType {
        [ClockParameters(4, ClockMode.Automatic)]
        MainClock,

        [ClockParameters(10000, ClockMode.Manual)]
        MainManualClock,

        [ClockParameters(50, ClockMode.Manual)]
        TestManualClock,
    }
}

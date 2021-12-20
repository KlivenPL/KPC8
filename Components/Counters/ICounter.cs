using Components.IODevices;
using Components.Signals;

namespace Components.Counters {
    public interface ICounter : IIODevice {
        public SignalPort LoadEnable { get; }
        public SignalPort OutputEnable { get; }
        public SignalPort CountEnable { get; }
        public SignalPort Clear { get; }
        public SignalPort Clk { get; }
    }
}

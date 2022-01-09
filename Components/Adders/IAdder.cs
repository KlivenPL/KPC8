using Components.IODevices;
using Components.Signals;

namespace Components.Adders {
    public interface IAdder : IIODevice {
        public SignalPort CarryIn { get; }
        public SignalPort ZeroFlag { get; }
        public SignalPort CarryFlag { get; }
        public SignalPort OverflowFlag { get; }
        public SignalPort NegativeFlag { get; }
        public SignalPort OutputEnable { get; }
        public SignalPort C { get; }
    }
}

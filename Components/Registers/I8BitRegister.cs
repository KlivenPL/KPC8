using Components.IODevices;
using Components.Signals;

namespace Components.Registers {
    public interface I8BitRegister : IIODevice {
        public SignalPort LoadEnable { get; }
        public SignalPort OutputEnable { get; }
        public SignalPort Clear { get; }
        public SignalPort Clk { get; }
    }
}

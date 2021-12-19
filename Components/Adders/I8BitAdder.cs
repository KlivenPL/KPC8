using Components.IODevices;
using Components.Signals;

namespace Components.Adders {
    public interface I8BitAdder : IIODevice {
        public SignalPort CarryIn { get; }
        public SignalPort CarryOut { get; }
        public SignalPort OutputEnable { get; }
        public SignalPort SubstractEnable { get; }
    }
}

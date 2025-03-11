using Components.IODevices;
using Components.Signals;

namespace Components.Rams {
    public interface IRam : IIODevice, Abstract.Components.IMemory {
        public SignalPort WriteEnable { get; }
        public SignalPort OutputEnable { get; }
        public SignalPort[] AddressInputs { get; }
        public SignalPort[] DataInputs { get; }
        SignalPort Clk { get; }
    }
}

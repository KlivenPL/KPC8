using Components.IODevices;
using Components.Signals;

namespace Components.Roms {
    public interface IRom : IIODevice {
        public SignalPort OutputEnable { get; }
        public SignalPort[] AddressInputs { get; }
    }
}

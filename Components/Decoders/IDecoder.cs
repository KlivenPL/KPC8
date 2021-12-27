using Components.IODevices;
using Components.Signals;

namespace Components.Decoders {
    public interface IDecoder : IIODevice {
        public SignalPort OutputEnable { get; }
    }
}

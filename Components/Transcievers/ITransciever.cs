using Components.IODevices;
using Components.Signals;

namespace Components.Transcievers {
    public interface ITransciever : IIODevice {
        public SignalPort OutputEnable { get; }
    }
}

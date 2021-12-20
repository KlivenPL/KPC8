using Components.Signals;

namespace Components.IODevices {
    public interface IIODevice {
        SignalPort[] Inputs { get; }
        SignalPort[] Outputs { get; }
    }
}

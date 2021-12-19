using Components.IODevices;
using Components.Signals;

namespace Components.Registers {
    public interface I8BitRegister : IIODevice {
        public SignalPort Inp1 { get; }
        public SignalPort Inp2 { get; }
        public SignalPort Inp3 { get; }
        public SignalPort Inp4 { get; }
        public SignalPort Inp5 { get; }
        public SignalPort Inp6 { get; }
        public SignalPort Inp7 { get; }
        public SignalPort Inp8 { get; }

        public SignalPort Out1 { get; }
        public SignalPort Out2 { get; }
        public SignalPort Out3 { get; }
        public SignalPort Out4 { get; }
        public SignalPort Out5 { get; }
        public SignalPort Out6 { get; }
        public SignalPort Out7 { get; }
        public SignalPort Out8 { get; }

        public SignalPort Load { get; }
        public SignalPort Enable { get; }
        public SignalPort Clear { get; }
        public SignalPort Clk { get; }
    }
}

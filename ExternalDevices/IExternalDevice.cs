using Components.Signals;

namespace ExternalDevices {
    public interface IExternalDevice {
        public SignalPort ChipSelect { get; set; }
        public SignalPort ExtIn { get; set; }
        public SignalPort ExtOut { get; set; }

        void ConnectAsExternalDevice(Signal extIn, Signal extOut, Signal chipSelect) {
            ChipSelect = new SignalPort(chipSelect);
            ExtIn = new SignalPort(extIn);
            ExtOut = new SignalPort(extOut);
            InitializeExternalDevice();
        }

        protected abstract void InitializeExternalDevice();
    }
}

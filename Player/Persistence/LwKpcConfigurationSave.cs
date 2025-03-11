using ExternalDevices.Lw;

namespace Player.Persistence {
    internal class LwKpcConfigurationSave {
        public LwKPadConfiguration[] KPads { get; set; }
        public LwKTimerConfiguration[] KTimers { get; set; }
    }
}

using LightweightEmulator.ExternalDevices;
using LightweightEmulator.Kpc;
using LightweightEmulator.Pipelines;

namespace LightweightEmulator.Configuration {
    public class LwKpcBuilder {
        private readonly LwKpcConfiguration _lwKpcConfiguration;

        public LwKpcBuilder(LwKpcConfiguration lwKpcConfiguration) {
            _lwKpcConfiguration = lwKpcConfiguration;
        }

        public LwKpcBuild Build() {
            var extDeviceAdapter = new LwExternalDevicesAdapter();
            var irrManager = new LwInterruptsManager();

            AddExternalDevices(extDeviceAdapter, irrManager);

            return new LwKpcBuild(_lwKpcConfiguration.RomData, _lwKpcConfiguration.InitialRamData, extDeviceAdapter, irrManager);
        }

        private void AddExternalDevices(LwExternalDevicesAdapter devicesAdapter, LwInterruptsManager irrManager) {
            if (_lwKpcConfiguration.DeviceConfigurations?.Any() != true) {
                return;
            }

            foreach (var extDeviceConfig in _lwKpcConfiguration.DeviceConfigurations) {
                if (extDeviceConfig is ILwKpcExternalDeviceConfiguration deviceConfig) {
                    deviceConfig.Configure(devicesAdapter.AddExternalDevice);
                } else if (extDeviceConfig is ILwKpcExternalInterruptDeviceConfiguration irrDeviceConfig) {
                    irrDeviceConfig.Configure(devicesAdapter.AddExternalDevice, irrManager.TryQueueInterrupt);
                }
            }
        }
    }
}

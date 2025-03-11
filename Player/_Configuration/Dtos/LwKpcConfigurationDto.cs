using LightweightEmulator.Configuration;
using Player.Persistence;

namespace Player._Configuration.Dtos {
    internal class LwKpcConfigurationDto {
        public byte[] InitialRamData { get; set; } = null;
        public IEnumerable<ILwKpcExternalDeviceConfigurationBase> DevicesConfigurations { get; set; } = null;

        public static LwKpcConfigurationDto FromSave(LwKpcConfigurationSave save) {

            var kpads = save.KPads ?? Array.Empty<ILwKpcExternalDeviceConfigurationBase>();
            var kTimers = save.KTimers ?? Array.Empty<ILwKpcExternalDeviceConfigurationBase>();

            var allDevices = new[] {
                kpads,
                kTimers
            };

            return new LwKpcConfigurationDto {
                DevicesConfigurations = allDevices.SelectMany(x => x).ToList(),
                InitialRamData = null,
            };
        }
    }
}

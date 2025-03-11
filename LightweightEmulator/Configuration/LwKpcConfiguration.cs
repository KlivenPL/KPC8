namespace LightweightEmulator.Configuration {
    public class LwKpcConfiguration {
        public byte[]? RomData { get; init; } = null;
        public byte[]? InitialRamData { get; init; } = null;
        public IEnumerable<ILwKpcExternalDeviceConfigurationBase>? DeviceConfigurations { get; init; }
    }
}

using LightweightEmulator.ExternalDevices;
using LightweightEmulator.Pipelines;

namespace LightweightEmulator.Configuration {
    public interface ILwKpcExternalDeviceConfigurationBase { }

    public interface ILwKpcExternalDeviceConfiguration : ILwKpcExternalDeviceConfigurationBase {
        void Configure(Action<ILwExternalDevice> addExternalDevice);
    }

    public interface ILwKpcExternalInterruptDeviceConfiguration
        : ILwKpcExternalDeviceConfigurationBase {
        void Configure(Action<ILwExternalDevice> addExternalDevice,
            TryQueueInterruptDelegate tryQueueInterrupt);
    }
}

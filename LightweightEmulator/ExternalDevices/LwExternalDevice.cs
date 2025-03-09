namespace LightweightEmulator.ExternalDevices {
    public interface ILwExternalDevice {
        bool IsAddressMapped(ushort address);
        void HandleSbext(ushort address, byte data);
        byte HandleLbext(ushort address);
    }

    public abstract class LwExternalDevice : ILwExternalDevice {
        public HashSet<ushort> MappedAddresses { get; init; } = new();

        public bool IsAddressMapped(ushort address) => MappedAddresses.Contains(address);

        public abstract void HandleSbext(ushort address, byte data);
        public abstract byte HandleLbext(ushort address);
    }
}

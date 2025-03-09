namespace LightweightEmulator.ExternalDevices {
    public class LwExternalDevicesAdapter {
        private readonly List<ILwExternalDevice> _externalDevices = new();

        public void AddExternalDevice(ILwExternalDevice externalDevice) {
            if (_externalDevices.Contains(externalDevice)) {
                throw new Exception("External device already added");
            }

            _externalDevices.Add(externalDevice);
        }

        public void HandleSbext(ushort address, byte data) {
            var mappedDevice = _externalDevices.SingleOrDefault(x => x.IsAddressMapped(address));
            mappedDevice?.HandleSbext(address, data);
        }

        public byte HandleLbext(ushort address) {
            var mappedDevice = _externalDevices.SingleOrDefault(x => x.IsAddressMapped(address));
            return mappedDevice?.HandleLbext(address) ?? 0;
        }
    }
}

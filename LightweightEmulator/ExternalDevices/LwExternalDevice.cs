using LightweightEmulator.Pipelines;

namespace LightweightEmulator.ExternalDevices {
    public interface ILwExternalDevice : IDisposable {
        bool IsAddressMapped(ushort address);
        void HandleSbext(ushort address, byte data);
        byte HandleLbext(ushort address);
    }

    public interface ILwExternalInterruptDevice : ILwExternalDevice {
        Task HandleInterruptReady();
    }

    public abstract class LwExternalDevice : ILwExternalDevice {
        protected LwExternalDevice(string name) {
            Name = name;
        }

        public string Name { get; }
        public HashSet<ushort> MappedAddresses { get; init; } = new();

        public bool IsAddressMapped(ushort address) => MappedAddresses.Contains(address);

        public abstract void Dispose();
        public virtual void HandleSbext(ushort address, byte data) { }
        public virtual byte HandleLbext(ushort address) => 0;

    }

    public abstract class LwExternalInterruptDevice : LwExternalDevice, ILwExternalInterruptDevice {
        private readonly TryQueueInterruptDelegate tryQueueInterrupt;

        protected LwExternalInterruptDevice(TryQueueInterruptDelegate tryQueueInterrupt, string name) : base(name) {
            this.tryQueueInterrupt = tryQueueInterrupt;
        }

        public abstract Task HandleInterruptReady();

        protected bool TryQueueInterrupt(byte fourBitIrrCode, out Action? abortIrrRequest) {
            return tryQueueInterrupt(fourBitIrrCode, HandleInterruptReady, out abortIrrRequest);
        }
    }
}

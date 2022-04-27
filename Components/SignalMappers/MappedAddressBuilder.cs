using System.Collections.Generic;

namespace Components.SignalMappers {
    public class MappedAddressBuilder {

        private readonly HashSet<ushort> mappedAddresses = new HashSet<ushort>();
        /*public MappedAddressBuilder Add(BitArray address) => Add(BitArrayHelper.ToUShortLE(address));
        public MappedAddressBuilder AddRange(BitArray addressFrom, BitArray addressTo) => AddRange(BitArrayHelper.ToUShortLE(addressFrom), BitArrayHelper.ToUShortLE(addressTo));
        public MappedAddressBuilder Remove(BitArray address) => Remove(BitArrayHelper.ToUShortLE(address));
        public MappedAddressBuilder RemoveRange(BitArray addressFrom, BitArray addressTo) => RemoveRange(BitArrayHelper.ToUShortLE(addressFrom), BitArrayHelper.ToUShortLE(addressTo));*/

        public MappedAddressBuilder Add(ushort address) {
            mappedAddresses.Add(address);
            return this;
        }

        public MappedAddressBuilder AddRange(ushort addressFrom, ushort addressTo) {
            for (ushort i = addressFrom; i <= addressTo; i++) {
                mappedAddresses.Add(i);
            }

            return this;
        }

        public MappedAddressBuilder Remove(ushort address) {
            mappedAddresses.Remove(address);
            return this;
        }

        public MappedAddressBuilder RemoveRange(ushort addressFrom, ushort addressTo) {
            for (ushort i = addressFrom; i <= addressTo; i++) {
                mappedAddresses.Remove(i);
            }

            return this;
        }

        public HashSet<ushort> Build() {
            return mappedAddresses;
        }
    }
}

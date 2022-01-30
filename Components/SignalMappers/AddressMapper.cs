using Components.Signals;
using Simulation.Updates;
using System;
using System.Collections.Generic;

namespace Components.SignalMappers {
    public class AddressMapper : IUpdate {
        private readonly HashSet<ushort> mappedAddresses;
        public SignalPort[] AddressInput;
        public SignalPort OutputEnable { get; } = new SignalPort();
        public Signal IsMatch { get; }
        public int Priority => -1;

        private AddressMapper(int addressSize, HashSet<ushort> mappedAddresses) {
            this.mappedAddresses = mappedAddresses;
            IsMatch = Signal.Factory.Create("AddressMapper");
            AddressInput = new SignalPort[addressSize];
            for (int i = 0; i < addressSize; i++) {
                AddressInput[i] = new SignalPort();
            }

            this.RegisterUpdate();
        }

        public static AddressMapper Create(int addressSize, Func<MappedAddressBuilder, MappedAddressBuilder> mappedAddressBuilder) {
            return new AddressMapper(addressSize, mappedAddressBuilder(new MappedAddressBuilder()).Build());
        }

        public void Update() {
            if (OutputEnable) {
                var address = GetOutputIndex();
                IsMatch.Value = mappedAddresses.Contains(address);
            } else {
                IsMatch.Value = false;
            }
        }

        private ushort GetOutputIndex() {
            var sum = 0;
            for (int i = AddressInput.Length - 1; i >= 0; i--) {
                sum += AddressInput[i] ? 1 << AddressInput.Length - i - 1 : 0;
            }

            return (ushort)sum;
        }

        public void Dispose() {
            this.UnregisterUpdate();
        }
    }
}

using Components.Signals;
using Simulation.Updates;
using System;
using System.Collections.Generic;

namespace Components.SignalMappers {
    public class AddressMapper : IUpdate {
        private readonly HashSet<int> mappedAddresses;
        public SignalPort[] AddressInput;
        public SignalPort OutputEnable { get; } = new SignalPort();
        public Signal IsMatch { get; }

        private AddressMapper(int addressSize, HashSet<int> mappedAddresses) {
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

        private int GetOutputIndex() {
            var sum = 0;
            for (int i = AddressInput.Length - 1; i >= 0; i--) {
                sum += AddressInput[i] ? 1 << AddressInput.Length - i - 1 : 0;
            }

            return sum;
        }

        public void Dispose() {
            this.UnregisterUpdate();
        }
    }
}

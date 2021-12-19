using System;

namespace Components.Signals {
    public sealed partial class Signal : IEquatable<Signal> {
        private readonly string name;
        private Signal masterSignal;
        private bool value;

        public event Action OnChange;

        private Signal(string name) {
            this.name = name;
        }

        public string Name => name;

        public static implicit operator bool(Signal signal) {
            return signal is not null && signal.Value;
        }

        public static implicit operator Readonly(Signal signal) {
            return new Readonly(signal);
        }

        public static bool operator ==(Signal sig1, Signal sig2) {
            if (sig1 is null && sig2 is null)
                return true;

            if (sig1 is null && sig2 is not null ||
                sig1 is not null && sig2 is null)
                return false;

            return sig1.Value == sig2.Value;
        }

        public static bool operator !=(Signal obj1, Signal obj2) {
            return !(obj1 == obj2);
        }

        public bool Equals(Signal obj) {
            if (obj == null)
                return false;

            return Value == obj.Value;
        }

        public override bool Equals(object obj) {
            return Equals(obj as Signal);
        }

        public override int GetHashCode() {
            return Value.GetHashCode();
        }

        public bool Value {
            get => masterSignal?.Value ?? value;
            set {
                if (masterSignal != null) {
                    throw new Exception("Cannot set value of slave's signal. Change the master signal instead.");
                }

                this.value = value;
                OnChange?.Invoke();
            }
        }

        public void SetMaster(Signal masterSignal) {
            if (this.masterSignal != null) {
                throw new Exception("Cannot set master signal, as master is already set.");
            }

            this.masterSignal = masterSignal;
        }
    }
}

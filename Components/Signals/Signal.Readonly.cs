using System;

namespace Components.Signals {
    public sealed partial class Signal {
        public class Readonly : IEquatable<Readonly>, IEquatable<Signal> {
            private readonly Signal signal;

            public bool Value => signal.Value;
            public string Name => signal.Name;

            public Readonly(string signalName) {
                signal = Factory.GetOrCreate(signalName);
            }

            public Readonly(Signal signal) {
                this.signal = signal;
            }

            public bool Equals(Readonly other) {
                return other.signal == signal;
            }

            public bool Equals(Signal other) {
                return signal == other;
            }

            public static bool operator ==(Readonly sig1, Readonly sig2) {
                if (sig1 is null || sig2 is null)
                    return false;

                return sig1.Equals(sig2);
            }

            public static bool operator !=(Readonly obj1, Readonly obj2) {
                return !(obj1 == obj2);
            }

            public override bool Equals(object obj) {
                return Equals(obj as Readonly);
            }

            public override int GetHashCode() {
                return signal.GetHashCode();
            }
        }
    }
}

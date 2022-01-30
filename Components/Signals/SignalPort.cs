using System;
using System.Linq;

namespace Components.Signals {
    public class SignalPort {
        public SignalPort() {
            signals = Array.Empty<Signal>();
        }

        public SignalPort(params Signal[] signals) : this() {
            foreach (var sig in signals) {
                PlugIn(sig);
            }
        }

        private Signal[] signals;

        public event Action OnEdgeRise;
        public event Action OnEdgeFall;

        public bool Value {
            get {
                for (int i = 0; i < signals.Length; i++) {
                    if (signals[i].Value)
                        return true;
                }

                return false;
            }
        }
        public void PlugIn(Signal signal) {
            signals = signals.Append(signal).ToArray();

            signal.OnChange += Signal_OnChange;
        }

        private void Signal_OnChange(Signal signal) {
            if (signal) {
                OnEdgeRise?.Invoke();
            } else {
                OnEdgeFall?.Invoke();
            }
        }

        public static implicit operator bool(SignalPort signalPort) {
            return signalPort is not null && signalPort.Value;
        }

        public void Write(bool value) {
            for (int i = 0; i < signals.Length; i++) {
                signals[i].Value = value;
            }
        }

        public override string ToString() {
            return Value ? "1" : "0";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Components.Signals {
    public class SignalPort {
        private List<Signal> signals = new List<Signal>();

        public event Action OnEdgeRise;
        public event Action OnEdgeFall;

        public bool Value {
            get {
                var anySig = signals.Count > 0;
                if (!anySig) {
                    Console.WriteLine("Trying to read unconnected port");
                }
                return anySig && signals.Any(s => s);
            }
        }
        public void PlugIn(Signal signal) {
            signals.Add(signal);

            signal.OnChange += Signal_OnChange;
        }

        public void UnplugAll() {
            if (!signals.Any()) {
                throw new Exception("This signal port was not occupied.");
            }

            signals.ForEach(s => s.OnChange -= Signal_OnChange);
            signals.Clear();
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
            if (signals.Any()) {
                signals.ForEach(s => s.Value = value);
            }
        }
    }
}

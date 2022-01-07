using System;
using System.Collections.Generic;
using System.Linq;

namespace Components.Signals {
    public class SignalPort {
        private List<Signal> signals = new List<Signal>();

        public event Action OnEdgeRise;
        /*public event Action OnEdgeFall;*/

        public bool Value {
            get {
                for (int i = 0; i < signals.Count; i++) {
                    if (signals[i].Value)
                        return true;
                }

                return false;
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
            }/* else {
                OnEdgeFall?.Invoke();
            }*/
        }

        public static implicit operator bool(SignalPort signalPort) {
            return signalPort is not null && signalPort.Value;
        }

        public void Write(bool value) {
            for (int i = 0; i < signals.Count; i++) {
                signals[i].Value = value;
            }
        }

        public override string ToString() {
            return Value ? "1" : "0";
        }
    }
}

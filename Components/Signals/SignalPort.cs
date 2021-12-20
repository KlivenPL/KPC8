using System;

namespace Components.Signals {
    public class SignalPort {
        private Signal signal;

        public event Action OnEdgeRise;
        public event Action OnEdgeFall;

        public bool Value => signal is not null && signal.Value;

        public void PlugIn(Signal signal) {
            if (this.signal != null) {
                throw new Exception("This signal port is already occupied.");
            }

            this.signal = signal;
            signal.OnChange += Signal_OnChange;
        }

        public void Unplug() {
            if (signal == null) {
                throw new Exception("This signal port was not occupied.");
            }

            signal.OnChange -= Signal_OnChange;
            signal = null;
        }

        private void Signal_OnChange() {
            if (signal == true) {
                OnEdgeRise?.Invoke();
            } else if (signal == false) {
                OnEdgeFall?.Invoke();
            }
        }

        public static implicit operator bool(SignalPort signalPort) {
            return signalPort is not null && signalPort.Value;
        }

        public void Write(bool value) {
            if (signal != null) {
                signal.Value = value;
            }
        }
    }
}

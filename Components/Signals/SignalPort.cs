using System;

namespace Components.Signals {
    public class SignalPort {
        private Signal signal;
        private bool prevSignalValue;

        public event Action OnEdgeRise;
        public event Action OnEdgeFall;

        public bool Value => signal is not null && signal.Value;

        public void PlugIn(Signal signal) {
            if (this.signal != null) {
                throw new Exception("This signal port is already occupied.");
            }

            this.signal = signal;
            prevSignalValue = signal;

            signal.OnChange += Signal_OnChange;
        }

        private void Signal_OnChange() {
            if (prevSignalValue != signal) {
                prevSignalValue = signal;

                if (signal == true) {
                    OnEdgeRise?.Invoke();
                } else if (signal == false) {
                    OnEdgeFall?.Invoke();
                }
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

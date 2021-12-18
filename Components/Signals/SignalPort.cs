using System;

namespace Components.Signals {
    class SignalPort {
        private Signal signal;

        public void PlugIn(Signal signal) {
            if (this.signal != null) {
                throw new Exception("This signal port is already occupied.");
            }

            this.signal = signal;
        }

        
    }
}

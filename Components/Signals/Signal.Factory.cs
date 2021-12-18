using System;
using System.Collections.Generic;
using System.Linq;

namespace Components.Signals {
    public sealed partial class Signal {
        public static class Factory {
            private static readonly List<Signal> signals = new List<Signal>();
            private static readonly HashSet<string> usedNames = new HashSet<string>();

            public static Signal GetOrCreate(string signalName) {
                var signal = signals.FirstOrDefault(s => s.name == signalName);

                if (signal != null) {
                    return signal;
                }

                if (usedNames.Contains(signalName)) {
                    throw new Exception($"Signal of name: {signalName} is already used");
                }

                usedNames.Add(signalName);
                return new Signal(signalName);
            }
        }
    }
}

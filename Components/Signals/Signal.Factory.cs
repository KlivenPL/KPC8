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

                signal = new Signal(signalName);

                signals.Add(signal);
                usedNames.Add(signalName);

                return signal;
            }

            public static void CreateAndConnectPorts(string namePrefix, IEnumerable<SignalPort> portsA, IEnumerable<SignalPort> portsB) {
                if (portsA.Count() != portsB.Count())
                    throw new Exception("Ports must be this same size");

                for (int i = 0; i < portsA.Count(); i++) {
                    CreateAndConnectPort($"{i}#{namePrefix}", portsA.ElementAt(i), portsB.ElementAt(i));
                }
            }

            public static void CreateAndConnectPort(string namePrefix, SignalPort portA, SignalPort portB) {
                var signal = Create(namePrefix);
                portA.PlugIn(signal);
                portB.PlugIn(signal);
            }

            public static Signal Create(string namePrefix) {
                return GetOrCreate($"{namePrefix}_{Guid.NewGuid()}");
            }
        }
    }
}

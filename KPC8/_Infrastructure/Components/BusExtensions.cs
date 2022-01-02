using Components.Buses;
using Components.Signals;
using KPC8.ControlSignals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KPC8._Infrastructure.Components {
    public static class BusExtensions {
        public static Signal ConnectAsControlSignal(this IBus bus, ControlSignalType signalType, SignalPort controllablePort) {
            var laneId = (int)Math.Log2((uint)signalType);
            var laneIdInverted = bus.Lanes.Length - laneId - 1;
            bus.Connect(laneIdInverted, controllablePort);
            return bus.Lanes[laneIdInverted];
        }

        public static Signal ConnectToControllerPort(this IBus bus, ControlSignalType signalType, IEnumerable<SignalPort> controllerPorts) {
            var laneId = (int)Math.Log2((uint)signalType);
            bus.Connect(laneId, controllerPorts.ElementAt(laneId));
            return bus.Lanes[laneId];
        }

        public static Signal GetControlSignal(this IBus bus, ControlSignalType signalType) {
            var laneId = (int)Math.Log2((uint)signalType);
            var laneIdInverted = bus.Lanes.Length - laneId - 1;
            return bus.Lanes[laneIdInverted];
        }
    }
}

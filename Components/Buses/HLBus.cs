using _Infrastructure.BitArrays;
using Components.Signals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Components.Buses {
    public class HLBus : IBus {
        public Signal[] Lanes { get; private set; }

        public HLBus(string name, int lanes) {
            Lanes = new Signal[lanes];

            for (int i = 0; i < lanes; i++) {
                Lanes[i] = Signal.Factory.GetOrCreate($"{name}_{Guid.NewGuid()}");
            }
        }

        public IBus Connect(int lane, SignalPort port) {
            port.PlugIn(Lanes[lane]);
            return this;
        }

        public IBus Connect(int startLane, int pinsCount, IEnumerable<SignalPort> ports) {
            for (int i = 0; i < pinsCount; i++) {
                var port = ports.ElementAt(i);
                port.PlugIn(Lanes[startLane++]);
            }

            return this;
        }

        public Signal.Readonly Peek(int lane) {
            return Lanes[lane];
        }

        public BitArray PeakAll() {
            return Lanes.ToBitArray();
        }
    }
}

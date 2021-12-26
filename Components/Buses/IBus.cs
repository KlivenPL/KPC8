using Components.Signals;
using System.Collections;
using System.Collections.Generic;

namespace Components.Buses {
    public interface IBus {
        Signal[] Lanes { get; }
        IBus Connect(int lane, SignalPort port);
        IBus Connect(int startLane, int pinsCount, IEnumerable<SignalPort> ports);
        BitArray PeakAll();
        Signal.Readonly Peek(int lane);
        void WriteTestOnly(BitArray data);
    }
}

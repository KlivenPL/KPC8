using Components.Buses;
using System.Collections;

namespace Components._Infrastructure.Components {
    public static class BusExtensions {
        public static void Write(this IBus bus, BitArray data) {
            for (int i = 0; i < data.Length; i++) {
                bus.Lanes[i].Value = data[i];
            }
        }
    }
}

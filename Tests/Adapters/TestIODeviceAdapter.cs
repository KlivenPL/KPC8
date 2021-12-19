using Components.IODevices;
using Components.Signals;
using System;
using System.Collections.Generic;

namespace Tests.Adapters {
    static class TestIODeviceAdapter {
        public static IEnumerable<Signal> CreateSignalAndPlugInInputs(this IIODevice device) {
            foreach (var input in device.Inputs) {
                var signal = CreateTestSignal();
                input.PlugIn(signal);
                yield return signal;
            }
        }

        public static IEnumerable<Signal> CreateSignalAndPlugInOutputs(this IIODevice device) {
            foreach (var output in device.Outputs) {
                Signal signal = CreateTestSignal();
                output.PlugIn(signal);
                yield return signal;
            }
        }

        public static Signal CreateSignalAndPlugInPort<T>(this T device, Func<T, SignalPort> portSelector) where T : IIODevice {
            var signal = CreateTestSignal();
            portSelector(device).PlugIn(signal);
            return signal;
        }

        private static Signal CreateTestSignal() {
            return Signal.Factory.GetOrCreate($"Test_{Guid.NewGuid()}");
        }
    }
}

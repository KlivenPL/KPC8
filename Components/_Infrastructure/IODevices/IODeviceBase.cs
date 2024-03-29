﻿using Components.IODevices;
using Components.Signals;

namespace Components._Infrastructure.IODevices {
    public class IODeviceBase : IIODevice {
        public SignalPort[] Inputs { get; private set; }
        public SignalPort[] Outputs { get; private set; }
        public string Name { get; }

        public IODeviceBase(string name) {
            Name = name;
        }

        public virtual void Initialize(int inputSize, int outputSize) {
            Inputs = new SignalPort[inputSize];
            for (int i = 0; i < inputSize; i++) {
                Inputs[i] = new SignalPort();
            }

            Outputs = new SignalPort[outputSize];
            for (int i = 0; i < outputSize; i++) {
                Outputs[i] = new SignalPort();
            }
        }

        public override string ToString() {
            return $"{Name}({this.GetType().Name})";
        }
    }
}

using Components._Infrastructure.IODevices;
using Components.Signals;
using Simulation.Updates;
using System;

namespace Components.Decoders {
    public class HLDecoder : IODeviceBase, IDecoder, IUpdate {
        public SignalPort OutputEnable { get; protected set; } = new SignalPort();

        public HLDecoder(int inputSize) {
            Initialize(inputSize);
            this.RegisterUpdate();
        }

        public void Initialize(int inputSize) {
            base.Initialize(inputSize, (int)Math.Pow(2, inputSize));
        }

        public void Update() {
            if (OutputEnable) {
                WriteOutput();
            }
        }

        private void WriteOutput() {
            var index = GetOutputIndex();
            for (int i = 0; i < Outputs.Length; i++) {
                var value = i == index;
                Outputs[i].Write(value);
            }
        }

        private int GetOutputIndex() {
            var sum = 0;
            for (int i = Inputs.Length - 1; i >= 0; i--) {
                sum += Inputs[i] ? 1 << Inputs.Length - i - 1 : 0;
            }

            return sum;
        }

        public void Dispose() {
            this.UnregisterUpdate();
        }
    }
}

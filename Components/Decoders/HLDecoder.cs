using Components._Infrastructure.IODevices;
using Components.Signals;
using Infrastructure.BitArrays;
using Simulation.Updates;
using System;
using System.Collections;

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
            Outputs[index].Write(true);
        }

        private int GetOutputIndex() {
            var inputs = new BitArray(Inputs.Length);
            for (int i = 0; i < inputs.Length; i++) {
                inputs[i] = Inputs[i];
            }

            return inputs.ToIntLittleEndian();
        }

        public void Dispose() {
            this.UnregisterUpdate();
        }
    }
}

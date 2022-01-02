using _Infrastructure.BitArrays;
using Components._Infrastructure.IODevices;
using Components.Signals;
using Infrastructure.BitArrays;
using Simulation.Updates;
using System;
using System.Collections;
using System.Text;

namespace Components.Decoders {
    public class HLDecoder : IODeviceBase, IDecoder, IUpdate {
        public SignalPort OutputEnable { get; protected set; } = new SignalPort();

        public BitArray Input => Inputs.ToBitArray();
        public BitArray Output => Outputs.ToBitArray();

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

        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine($"Input: {Input.ToPrettyBitString()}");
            sb.AppendLine($"Output: {Output.ToPrettyBitString()}");

            return sb.ToString();
        }
    }
}

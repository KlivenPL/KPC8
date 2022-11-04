using _Infrastructure.BitArrays;
using Components._Infrastructure.IODevices;
using Components.Signals;
using Infrastructure.BitArrays;
using Simulation.Updates;
using System.Linq;
using System.Text;

namespace Components.Multiplexers {
    public class HLSingleSwitch2NToNMux : IODeviceBase, IMux, IUpdate {
        public SignalPort SelectB { get; protected set; } = new SignalPort();
        public SignalPort[] InputsA => Inputs.Take(singleInputSize).ToArray();
        public SignalPort[] InputsB => Inputs.Skip(singleInputSize).ToArray();
        public SignalPort Clk { get; protected set; } = new SignalPort();

        private readonly int singleInputSize;

        public HLSingleSwitch2NToNMux(string name, int singleInputSize) : base(name) {
            this.singleInputSize = singleInputSize;

            Initialize();
            this.RegisterUpdate();
        }

        public void Initialize() {
            base.Initialize(singleInputSize * 2, singleInputSize);
        }

        public void Update() {
            if (Clk) {
                WriteOutput();
            }
        }

        private void WriteOutput() {
            var offset = SelectB ? singleInputSize : 0;

            for (int i = 0; i < Outputs.Length; i++) {
                Outputs[i].Write(Inputs[i + offset]);
            }
        }

        public void Dispose() {
            this.UnregisterUpdate();
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.AppendLine($"SelectB: {(SelectB ? "1" : "0")}, Outputs: {Outputs.ToBitArray().ToPrettyBitString()}");

            return sb.ToString();
        }
    }
}

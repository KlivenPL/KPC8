using _Infrastructure.BitArrays;
using Components._Infrastructure.IODevices;
using Components.Signals;
using Infrastructure.BitArrays;
using Simulation.Updates;
using System.Collections;
using System.Linq;
using System.Text;

namespace Components.Adders {
    public class HLAdder : IODeviceBase, IAdder, IUpdate {
        private readonly BitArray inputA;
        private readonly BitArray inputB;

        public BitArray Content => new(inputA);
        public SignalPort CarryIn { get; protected set; } = new SignalPort();
        public SignalPort CarryFlag { get; protected set; } = new SignalPort();
        public SignalPort OutputEnable { get; protected set; } = new SignalPort();
        public SignalPort SubstractEnable { get; protected set; } = new SignalPort();
        public SignalPort ZeroFlag { get; protected set; } = new SignalPort();
        public SignalPort OverflowFlag { get; protected set; } = new SignalPort();
        public SignalPort NegativeFlag { get; protected set; } = new SignalPort();

        public SignalPort[] InputsA => Inputs.Take(inputA.Length).ToArray();
        public SignalPort[] InputsB => Inputs.TakeLast(inputA.Length).ToArray();

        public HLAdder(string name, int size) : base(name) {
            inputA = new(size);
            inputB = new(size);

            Initialize(size);
            this.RegisterUpdate();
        }

        public void Initialize(int size) {
            base.Initialize(size + size, size);
        }

        public void Update() {
            if (OutputEnable) {
                LoadInputs(SubstractEnable);

                Add(CarryIn || SubstractEnable, out var carryOut, out var isZero);
                WriteFlags(carryOut, isZero);

                WriteOutput();
            }
        }

        private void LoadInputs(bool invert) {
            for (int i = 0; i < inputA.Length; i++) {
                inputA[i] = Inputs[i];
                inputB[i] = invert ? !Inputs[i + inputA.Length] : Inputs[i + inputA.Length];
            }
        }

        private void Add(bool carryIn, out bool carryOut, out bool isZero) {
            isZero = true;

            for (int i = inputA.Length - 1; i >= 0; i--) {
                var tmp = inputA[i] ^ inputB[i] ^ carryIn;
                carryIn = inputA[i] && inputB[i] || inputA[i] && carryIn || inputB[i] && carryIn;
                inputA[i] = tmp;

                if (tmp) {
                    isZero = false;
                }
            }

            carryOut = carryIn;
        }

        private void WriteOutput() {
            for (int i = 0; i < inputA.Length; i++) {
                Outputs[i].Write(inputA[i]);
            }
        }

        /// <summary>
        /// http://teaching.idallen.com/dat2343/10f/notes/040_overflow.txt
        /// </summary>
        private void WriteFlags(bool carryOut, bool isZero) {
            CarryFlag.Write(carryOut);
            NegativeFlag.Write(inputA[0]);

            var overflow =
                SubstractEnable ?
                    !Inputs[0] && Inputs[inputA.Length] && inputA[0] ||
                    Inputs[0] && !Inputs[inputA.Length] && !inputA[0]
                :
                    !Inputs[0] && !inputB[0] && inputA[0] ||
                    Inputs[0] && inputB[0] && !inputA[0];

            OverflowFlag.Write(overflow);
            ZeroFlag.Write(isZero);
        }

        public void Dispose() {
            this.UnregisterUpdate();
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.AppendLine($"InputA: {InputsA.ToBitArray().ToPrettyBitString()}");
            sb.AppendLine($"InputB: {InputsB.ToBitArray().ToPrettyBitString()}");
            sb.AppendLine($"Content: {Content.ToPrettyBitString()}");

            return sb.ToString();
        }
    }
}

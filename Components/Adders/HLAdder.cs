using _Infrastructure.BitArrays;
using _Infrastructure.Simulation.Updates;
using Components._Infrastructure.IODevices;
using Components.Signals;
using Infrastructure.BitArrays;
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
        public SignalPort A { get; protected set; } = new SignalPort();
        public SignalPort B { get; protected set; } = new SignalPort();
        public SignalPort C { get; protected set; } = new SignalPort();
        public SignalPort ZeroFlag { get; protected set; } = new SignalPort();
        public SignalPort OverflowFlag { get; protected set; } = new SignalPort();
        public SignalPort NegativeFlag { get; protected set; } = new SignalPort();

        public SignalPort[] InputsA => Inputs.Take(inputA.Length).ToArray();
        public SignalPort[] InputsB => Inputs.TakeLast(inputA.Length).ToArray();

        enum SubOperation {
            None,
            Subtract,
            Not,
            Or,
            And,
            Xor,
            Sl,
            Sr,
        }

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
                var subOperation = GetSubOperation();
                var isSubtraction = subOperation == SubOperation.Subtract;

                LoadInputs(invert: isSubtraction);

                bool effectiveCarryIn = CarryIn || isSubtraction;

                Operate(subOperation, effectiveCarryIn, out var carryOut, out var isZero);

                WriteFlags(isSubtraction, carryOut, isZero);
                WriteOutput();
            }
        }

        private void LoadInputs(bool invert) {
            for (int i = 0; i < inputA.Length; i++) {
                inputA[i] = Inputs[i];
                inputB[i] = invert ? !Inputs[i + inputA.Length] : Inputs[i + inputA.Length];
            }
        }

        private void Operate(SubOperation subOperation, bool carryIn, out bool carryOut, out bool isZero) {
            carryOut = false;
            isZero = true;

            if (subOperation != SubOperation.Or && subOperation != SubOperation.And && subOperation != SubOperation.Xor) {

                for (int i = inputA.Length - 1; i >= 0; i--) {
                    var tmp = inputA[i] ^ inputB[i] ^ carryIn;

                    if (subOperation == SubOperation.Not) {
                        tmp = !tmp;
                    }

                    carryIn = inputA[i] && inputB[i] || inputA[i] && carryIn || inputB[i] && carryIn;
                    inputA[i] = tmp;

                    if (tmp) {
                        isZero = false;
                    }
                }

                carryOut = carryIn;
            }

            switch (subOperation) {
                case SubOperation.Or:
                    for (int i = inputA.Length - 1; i >= 0; i--) {
                        inputA[i] = inputA[i] | inputB[i];

                        if (inputA[i])
                            isZero = false;
                    }
                    return;
                case SubOperation.And:
                    for (int i = inputA.Length - 1; i >= 0; i--) {
                        inputA[i] = inputA[i] & inputB[i];

                        if (inputA[i])
                            isZero = false;
                    }
                    return;
                case SubOperation.Xor:
                    for (int i = inputA.Length - 1; i >= 0; i--) {
                        inputA[i] = inputA[i] ^ inputB[i];

                        if (inputA[i])
                            isZero = false;
                    }
                    return;
                case SubOperation.Sl:
                    bool prev = false;
                    for (int i = inputA.Length - 1; i >= 0; i--) {
                        var curr = inputA[i];
                        inputA[i] = prev;
                        prev = curr;

                        if (inputA[i])
                            isZero = false;
                    }
                    return;
                case SubOperation.Sr:
                    prev = false;
                    for (int i = 0; i < inputA.Length; i++) {
                        var curr = inputA[i];
                        inputA[i] = prev;
                        prev = curr;

                        if (inputA[i])
                            isZero = false;
                    }
                    return;

                default:
                    return;
            }
        }

        private void WriteOutput() {
            for (int i = 0; i < inputA.Length; i++) {
                Outputs[i].Write(inputA[i]);
            }
        }

        /// <summary>
        /// http://teaching.idallen.com/dat2343/10f/notes/040_overflow.txt
        /// </summary>
        private void WriteFlags(bool isSubtraction, bool carryOut, bool isZero) {
            CarryFlag.Write(carryOut);
            NegativeFlag.Write(inputA[0]);

            var overflow =
                isSubtraction ?
                    !Inputs[0] && Inputs[inputA.Length] && inputA[0] ||
                    Inputs[0] && !Inputs[inputA.Length] && !inputA[0]
                :
                    !Inputs[0] && !inputB[0] && inputA[0] ||
                    Inputs[0] && inputB[0] && !inputA[0];


            OverflowFlag.Write(overflow);
            ZeroFlag.Write(isZero);
        }

        private SubOperation GetSubOperation() {
            bool a = A, b = B, c = C;

            if (!a && !b && !c) {
                return SubOperation.None;
            } else if (!a && !b && c) {
                return SubOperation.Subtract;
            } else if (!a && b && !c) {
                return SubOperation.Not;
            } else if (!a && b && c) {
                return SubOperation.Or;
            } else if (a && !b && !c) {
                return SubOperation.And;
            } else if (a && !b && c) {
                return SubOperation.Xor;
            } else if (a && b && !c) {
                return SubOperation.Sl;
            } else if (a && b && c) {
                return SubOperation.Sr;
            }

            return SubOperation.None;
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

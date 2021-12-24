using Components._Infrastructure.IODevices;
using Components.Signals;
using Simulation.Updates;
using System.Collections;

namespace Components.Adders {
    public class HLAdder : IODeviceBase, IAdder, IUpdate {
        private readonly BitArray inputA;
        private readonly BitArray inputB;

        public BitArray Content => new(inputA);
        public SignalPort CarryIn { get; protected set; } = new SignalPort();
        public SignalPort CarryOut { get; protected set; } = new SignalPort();
        public SignalPort OutputEnable { get; protected set; } = new SignalPort();
        public SignalPort SubstractEnable { get; protected set; } = new SignalPort();

        public HLAdder(int size) {
            inputA = new(size);
            inputB = new(size);

            Initialize(size);
            this.RegisterUpdate();
        }

        public void Initialize(int size) {
            base.Initialize(size + size, size);
        }

        public void Update() {
            LoadInputA();
            LoadInputB(SubstractEnable);

            CarryOut.Write(Add(CarryIn || SubstractEnable));

            if (OutputEnable) {
                WriteOutput();
            }
        }

        private void LoadInputA() {
            for (int i = 0; i < inputA.Length; i++) {
                inputA[i] = Inputs[i];
            }
        }

        private void LoadInputB(bool invert) {
            for (int i = 0; i < inputA.Length; i++) {
                inputB[i] = invert ? !Inputs[i + inputA.Length] : Inputs[i + inputA.Length];
            }
        }

        private bool Add(bool carryIn) {
            for (int i = inputA.Length - 1; i >= 0; i--) {
                var tmp = inputA[i] ^ inputB[i] ^ carryIn;
                carryIn = inputA[i] && inputB[i] || inputA[i] && carryIn || inputB[i] && carryIn;
                inputA[i] = tmp;
            }

            return carryIn;
        }

        private void WriteOutput() {
            for (int i = 0; i < inputA.Length; i++) {
                Outputs[i].Write(inputA[i]);
            }
        }

        public void Dispose() {
            this.UnregisterUpdate();
        }
    }
}

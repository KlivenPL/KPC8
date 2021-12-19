using Components.IODevices;
using Components.Signals;
using Simulation.Updates;
using System.Collections;

namespace Components.Adders {
    class HL8BitAdder : IODeviceBase, I8BitAdder, IUpdate {
        private readonly BitArray inputA = new(8);
        private readonly BitArray inputB = new(8);

        //private readonly BitArray result = new(8);

        public BitArray Content => new(inputA);
        public SignalPort CarryIn { get; protected set; } = new SignalPort();
        public SignalPort CarryOut { get; protected set; } = new SignalPort();
        public SignalPort OutputEnable { get; protected set; } = new SignalPort();
        public SignalPort SubstractEnable { get; protected set; } = new SignalPort();

        public HL8BitAdder() {
            Initialize();
            this.RegisterUpdate();
        }

        public void Initialize() {
            base.Initialize(16, 8);
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
            for (int i = 0; i < 8; i++) {
                inputA[i] = Inputs[i];
            }
        }

        private void LoadInputB(bool invert) {
            for (int i = 0; i < 8; i++) {
                inputB[i] = invert ? !Inputs[i + 8] : Inputs[i + 8];
            }
        }

        private bool Add(bool carryIn) {
            for (int i = 7; i >= 0; i--) {
                var tmp = inputA[i] ^ inputB[i] ^ carryIn;
                carryIn = inputA[i] && inputB[i] || inputA[i] && carryIn || inputB[i] && carryIn;
                inputA[i] = tmp;
            }

            return carryIn;
        }

        private void WriteOutput() {
            for (int i = 0; i < 8; i++) {
                Outputs[i].Write(inputA[i]);
            }
        }
    }
}

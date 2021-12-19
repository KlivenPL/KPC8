using System.Collections;

namespace Components.Registers {
    class HL8BitRegister : HL8BitRegisterBase, I8BitRegister {
        private readonly BitArray mainBuffer = new(8);

        public BitArray Content => new(mainBuffer);

        public HL8BitRegister() {
            Initialize();
        }

        public override void Initialize() {
            base.Initialize();

            Clk.OnEdgeRise += Clk_OnEdgeRise;
            Clear.OnEdgeRise += Clear_OnEdgeRise;
        }

        private void Clear_OnEdgeRise() {
            mainBuffer.SetAll(false);
        }

        private void Clk_OnEdgeRise() {
            if (Enable) {
                WriteOutput();
            } else if (Load) {
                LoadInput();
            }
        }

        private void LoadInput() {
            for (int i = 0; i < 8; i++) {
                mainBuffer[i] = Inputs[i];
            }
        }

        private void WriteOutput() {
            for (int i = 0; i < 8; i++) {
                Outputs[i].Write(mainBuffer[i]);
            }
        }
    }
}

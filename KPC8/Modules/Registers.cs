using Components.Buses;
using Components.Registers;
using Components.Signals;
using Infrastructure.BitArrays;
using KPC8._Infrastructure.Components;
using KPC8.ControlSignals;
using KPC8.ProgRegs;
using System;
using System.Collections;
using System.Linq;

namespace KPC8.Modules {
    public class Registers : ModuleBase<CsPanel.RegsPanel> {
        private const int RegistersCount = 16;
        private const int RegisterSize = 16;

        private readonly HLLohRegister[] regs;

        public BitArray GetWholeRegContent(int index) => regs[index].Content;
        public BitArray GetLoRegContent(int index) => regs[index].Content.Skip(8);
        public BitArray GetHiRegContent(int index) => regs[index].Content.Take(8);
        public void SetWholeRegContent(int index, BitArray value) => regs[index].SetContent(value);
        public bool IsRegSelected(int index) => regs[index].ChipEnable;

        public Registers(Signal mainClock, IBus dataBus, IBus registerSelectBus) {
            regs = new HLLohRegister[RegistersCount];

            for (int i = 0; i < RegistersCount; i++) {
                regs[i] = new HLLohRegister(nameof(regs), RegisterSize);
            }

            ConnectMainClock(mainClock);
            ConnectDataBus(dataBus);
            ConnectRegisterSelectBus(registerSelectBus);
        }

        protected override void ConnectMainClock(Signal mainClock) {
            for (int i = 0; i < RegistersCount; i++) {
                regs[i].Clk.PlugIn(mainClock);
            }
        }

        protected override void ConnectDataBus(IBus dataBus) {
            dataBus.Connect(0, 8, regs[0].Outputs.Take(8));
            dataBus.Connect(0, 8, regs[0].Outputs.TakeLast(8));

            for (int i = 1; i < RegistersCount; i++) {
                dataBus
                    .Connect(0, 8, regs[i].Inputs.Take(8))
                    .Connect(0, 8, regs[i].Inputs.TakeLast(8))

                    .Connect(0, 8, regs[i].Outputs.Take(8))
                    .Connect(0, 8, regs[i].Outputs.TakeLast(8));
            }
        }

        protected override void ConnectRegisterSelectBus(IBus registerSelectBus) {
            /*for (int i = 0; i < RegistersCount; i++) {
                var index = (int)Math.Log2();
                registerSelectBus.Connect(i, regs[i].ChipEnable);
            }*/

            foreach (var reg in Enum.GetValues<Regs>().Skip(1)) {
                var index = reg.GetIndex();
                registerSelectBus.Connect(index, regs[index].ChipEnable);
            }
        }

        public override CsPanel.RegsPanel CreateControlPanel(IBus controlBus) {
            var regs_L_controlSignal = controlBus.GetControlSignal(ControlSignalType.Regs_L);
            var regs_O_controlSignal = controlBus.GetControlSignal(ControlSignalType.Regs_O);
            var regs_H_controlSignal = controlBus.GetControlSignal(ControlSignalType.Regs_H);

            for (int i = 0; i < RegistersCount; i++) {
                regs[i].L.PlugIn(regs_L_controlSignal);
                regs[i].O.PlugIn(regs_O_controlSignal);
                regs[i].H.PlugIn(regs_H_controlSignal);
            }

            return new CsPanel.RegsPanel {
                Regs_L = regs_L_controlSignal,
                Regs_O = regs_O_controlSignal,
                Regs_H = regs_H_controlSignal
            };
        }

    }
}

using Components.Buses;
using Components.Clocks;
using Components.Registers;
using KPC8._Infrastructure.Components;
using KPC8.ControlSignals;
using KPC8.ProgRegs;
using System;
using System.Collections;
using System.Linq;

namespace KPC8.Modules {
    public class Registers : ModuleBase<CsPanel.RegsPanel> {
        private const int RegistersCount = 16;
        private const int RegisterSize = 8;

        private readonly HLCeRegister[] regs;

        public BitArray GetRegContent(int index) => regs[index].Content;
        public bool IsRegSelected(int index) => regs[index].ChipEnable;

        public Registers(Clock mainClock, IBus dataBus, IBus registerSelectBus) {
            regs = new HLCeRegister[RegistersCount];

            for (int i = 0; i < RegistersCount; i++) {
                regs[i] = new HLCeRegister(RegisterSize);
            }

            ConnectMainClock(mainClock);
            ConnectDataBus(dataBus);
            ConnectRegisterSelectBus(registerSelectBus);
        }

        protected override void ConnectMainClock(Clock mainClock) {
            for (int i = 0; i < RegistersCount; i++) {
                regs[i].Clk.PlugIn(mainClock.Clk);
            }
        }

        protected override void ConnectDataBus(IBus dataBus) {
            dataBus.Connect(0, 8, regs[0].Outputs);

            for (int i = 1; i < RegistersCount; i++) {
                dataBus
                    .Connect(0, 8, regs[i].Inputs)
                    .Connect(0, 8, regs[i].Outputs);
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
            var regs_le_controlSignal = controlBus.GetControlSignal(ControlSignalType.Regs_le);
            var regs_oe_controlSignal = controlBus.GetControlSignal(ControlSignalType.Regs_oe);

            for (int i = 0; i < RegistersCount; i++) {
                regs[i].LoadEnable.PlugIn(regs_le_controlSignal);
                regs[i].OutputEnable.PlugIn(regs_oe_controlSignal);
            }

            return new CsPanel.RegsPanel {
                Regs_le = regs_le_controlSignal,
                Regs_oe = regs_oe_controlSignal
            };
        }

    }
}

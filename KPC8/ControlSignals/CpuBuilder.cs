using Components.Buses;
using Components.Clocks;
using KPC8.Modules;
using System.Collections;

namespace KPC8.ControlSignals {
    public class CpuBuilder {
        private System.Func<CsPanel.MemoryPanel> createMemoryPanel;
        private System.Func<CsPanel.ControlPanel> createControlPanel;
        private System.Func<CsPanel.RegsPanel> createRegsPanel;
        private System.Func<CsPanel.AluPanel> createAluPanel;

        private IBus dataBus;
        private IBus addressBus;
        private IBus flagsBus;
        private IBus registerSelectBus;
        private IBus controlBus;

        private readonly Clock mainClock;

        public CpuBuilder(Clock mainClock) {
            this.mainClock = mainClock;
        }

        public CpuBuilder WithMemoryModule(BitArray[] romData, BitArray[] ramData) {
            createMemoryPanel = () => {
                var memory = new Memory(romData, ramData, mainClock.Clk, dataBus, addressBus);
                return memory.CreateControlPanel(controlBus);
            };

            return this;
        }

        public CpuBuilder WithControlModule(BitArray[] instRomData, bool connectControlBusToControllerPorts) {
            createControlPanel = () => {
                var control = new Control(instRomData, mainClock.Clk, dataBus, registerSelectBus, flagsBus); // TODO poprawic na main clock bar

                if (connectControlBusToControllerPorts) {
                    control.ConnectControlBusToControllerPorts(controlBus);
                }

                return control.CreateControlPanel(controlBus);
            };

            return this;
        }

        public CpuBuilder WithRegistersModule() {
            createRegsPanel = () => {
                var control = new Registers(mainClock.Clk, dataBus, registerSelectBus);
                return control.CreateControlPanel(controlBus);
            };

            return this;
        }

        public CpuBuilder WithAluModule() {
            createAluPanel = () => {
                var alu = new Alu(mainClock.Clk, dataBus, flagsBus);
                return alu.CreateControlPanel(controlBus);
            };

            return this;
        }

        public CsPanel Build() {
            dataBus = new HLBus("TestDataBus", 8);
            flagsBus = new HLBus("FlagsBus", 4);
            controlBus = new HLBus("ControlBus", 32);
            registerSelectBus = new HLBus("RegisterSelectBus", 16);
            addressBus = new HLBus("TestAddressBus", 16);

            return new CsPanel {
                Mem = createMemoryPanel?.Invoke(),
                Ctrl = createControlPanel?.Invoke(),
                Regs = createRegsPanel?.Invoke(),
                Alu = createAluPanel?.Invoke()
            };
        }
    }
}

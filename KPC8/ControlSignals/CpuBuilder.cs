using Components.Buses;
using Components.Clocks;
using KPC8.Microcode;
using KPC8.Modules;
using KPC8.RomProgrammers.Microcode;
using System.Collections;
using System.Linq;

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
        private IBus interruptsBus;

        private readonly Clock mainClock;
        private readonly ModulePanel modules;

        public CpuBuilder(Clock mainClock) {
            this.mainClock = mainClock;
            this.modules = new ModulePanel();
        }

        public CpuBuilder WithMemoryModule(BitArray[] romData, BitArray[] ramData) {
            createMemoryPanel = () => {
                var memory = new Memory(romData, ramData, mainClock.Clk, dataBus, addressBus);
                modules.Memory = memory;
                return memory.CreateControlPanel(controlBus);
            };
            return this;
        }

        public CpuBuilder WithControlModule(BitArray[] customInstRomData, bool connectControlBusToControllerPorts) {
            createControlPanel = () => {

                var instRomData = customInstRomData ?? new McRomBuilder(64)
                    .FindAndAddAllProceduralInstructions()
                    .SetDefaultInstruction(new McProceduralInstruction("NOP", NopInstruction.Nop().ToArray(), 0x0))
                    .Build();

                var control = new Control(instRomData, mainClock.ClkBar, dataBus, registerSelectBus, flagsBus, interruptsBus);

                if (connectControlBusToControllerPorts) {
                    control.ConnectControlBusToControllerPorts(controlBus);
                }

                modules.Control = control;

                return control.CreateControlPanel(controlBus);
            };

            return this;
        }

        public CpuBuilder WithRegistersModule() {
            createRegsPanel = () => {
                var registers = new Registers(mainClock.Clk, dataBus, registerSelectBus);
                modules.Registers = registers;
                return registers.CreateControlPanel(controlBus);
            };

            return this;
        }

        public CpuBuilder WithAluModule() {
            createAluPanel = () => {
                var alu = new Alu(mainClock.Clk, dataBus, flagsBus);
                modules.Alu = alu;
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
            interruptsBus = new HLBus("TestInterruptsBus", 8);

            return new CsPanel {
                Mem = createMemoryPanel?.Invoke(),
                Ctrl = createControlPanel?.Invoke(),
                Regs = createRegsPanel?.Invoke(),
                Alu = createAluPanel?.Invoke()
            };
        }

        public CsPanel BuildWithModulesAccess(out ModulePanel modules) {
            var csPanel = Build();
            modules = this.modules;
            modules.AddressBus = addressBus;
            modules.ControlBus = controlBus;
            modules.DataBus = dataBus;
            modules.FlagsBus = flagsBus;
            modules.RegisterSelectBus = registerSelectBus;
            modules.InterruptsBus = interruptsBus;

            return csPanel;
        }
    }
}

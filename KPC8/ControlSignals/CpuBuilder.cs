using Components.Buses;
using Components.Clocks;
using KPC8._Infrastructure.Components;
using KPC8.ExternalModules;
using KPC8.Microcode;
using KPC8.Modules;
using KPC8.RomProgrammers.Microcode;
using Simulation.Loops;
using System;
using System.Collections;
using System.Linq;

namespace KPC8.ControlSignals {
    public class CpuBuilder {
        private Func<CsPanel.MemoryPanel> createMemoryPanel;
        private Func<CsPanel.ControlPanel> createControlPanel;
        private Func<CsPanel.RegsPanel> createRegsPanel;
        private Func<CsPanel.AluPanel> createAluPanel;
        private Action postBuildActions;
        private bool isBaseBuilt = false;

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

                var instRomData = customInstRomData ?? new McRomBuilder(64 + 8)
                    .FindAndAddAllProceduralInstructions()
                    .FindAndAddAllConditionalInstructions()
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

        public CpuBuilder PostBuild(Func<PostBuildActionsBuilder, PostBuildActionsBuilder> postBuildActionsBuilder) {
            postBuildActions += () => {
                postBuildActionsBuilder(new PostBuildActionsBuilder(this)).Execute();
            };

            return this;
        }

        public CpuBuilder InNewSimulationLoop(out Func<SimulationLoop> getSimulationLoop, Func<SimulationLoopBuilder, SimulationLoopBuilder> newLoopBuilder, Func<PostBuildActionsBuilder, PostBuildActionsBuilder> postBuildActionsBuilder) {
            var simLoopBuilder = newLoopBuilder(SimulationLoopBuilder.Create());
            getSimulationLoop = () => {
                if (!isBaseBuilt) {
                    throw new Exception("Cannot get simulation loop as given builder has not built CPU yet");
                }
                return simLoopBuilder.Build();
            };

            postBuildActions += () => {
                if (SimulationLoopBuilder.Current == simLoopBuilder) {
                    throw new Exception("Simulation loop builder in this context should not be set as current manually.");
                }

                using (simLoopBuilder.SetAsCurrent()) {
                    postBuildActionsBuilder(new PostBuildActionsBuilder(this)).Execute();
                }
            };

            return this;
        }

        public class PostBuildActionsBuilder {
            private readonly CpuBuilder cpuBuilder;
            private Action additionalActions;

            public PostBuildActionsBuilder(CpuBuilder cpuBuilder) {
                if (!cpuBuilder.isBaseBuilt) {
                    throw new Exception("Cannot create as given builder has not built CPU yet");
                }

                this.cpuBuilder = cpuBuilder;
            }

            public PostBuildActionsBuilder Connect(Action<IBus, IBus> connectToDataAndAddressBus) {
                additionalActions += () => {
                    connectToDataAndAddressBus(cpuBuilder.dataBus, cpuBuilder.addressBus);
                };

                return this;
            }

            /*public PostBuildActionsBuilder ConnectExternalDevice(IExternalDevice externalDevice, Signal chipSelect) {
                additionalActions += () => {
                    var extIn = cpuBuilder.controlBus.GetControlSignal(ControlSignalType.Ext_in);
                    var extOut = cpuBuilder.controlBus.GetControlSignal(ControlSignalType.Ext_out);

                    externalDevice.ConnectAsExternalDevice(cpuBuilder.dataBus, cpuBuilder.addressBus, extIn, extOut, chipSelect);
                };

                return this;
            }

            public PostBuildActionsBuilder ConnectExternalDevice(IExternalDevice externalDevice, AddressMapper addressMapper) {
                return ConnectExternalDevice(externalDevice, addressMapper.IsMatch);
            }*/

            public PostBuildActionsBuilder ConnectExternalModule(ExternalModuleBase externalModule) {
                additionalActions += () => {
                    var extIn = cpuBuilder.controlBus.GetControlSignal(ControlSignalType.Ext_in);
                    var extOut = cpuBuilder.controlBus.GetControlSignal(ControlSignalType.Ext_out);

                    externalModule.InitializeAndConnect(cpuBuilder.dataBus, cpuBuilder.addressBus, cpuBuilder.interruptsBus, extIn, extOut);
                };

                return this;
            }

            public void Execute() {
                additionalActions?.Invoke();
            }
        }

        public CsPanel Build() {
            if (isBaseBuilt) {
                throw new Exception("This builder has already been used.");
            }

            dataBus = new HLBus("DataBus", 8);
            flagsBus = new HLBus("FlagsBus", 4);
            controlBus = new HLBus("ControlBus", 40);
            registerSelectBus = new HLBus("RegisterSelectBus", 16);
            addressBus = new HLBus("AddressBus", 16);
            interruptsBus = new HLBus("InterruptsBus", 8);

            var csPanel = new CsPanel {
                Mem = createMemoryPanel?.Invoke(),
                Ctrl = createControlPanel?.Invoke(),
                Regs = createRegsPanel?.Invoke(),
                Alu = createAluPanel?.Invoke(),
                Modifier = controlBus.GetControlSignal(ControlSignalType.MODIFIER),
            };

            isBaseBuilt = true;
            postBuildActions?.Invoke();

            return csPanel;
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

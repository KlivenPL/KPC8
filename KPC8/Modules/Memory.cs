using _Infrastructure.BitArrays;
using Components.Buses;
using Components.Clocks;
using Components.Counters;
using Components.Logic;
using Components.Rams;
using Components.Roms;
using Components.Signals;
using Components.Transcievers;
using KPC8._Infrastructure.Components;
using KPC8.ControlSignals;
using System.Collections;
using System.Linq;

namespace KPC8.Modules {
    public class Memory : ModuleBase<CsPanel.MemoryPanel> {
        private const int MemorySize = 65536; // 2 ^ 16

        private readonly HLHiLoCounter pc;
        private readonly HLHiLoCounter mar;
        private readonly HLRam ram;
        private readonly HLRom rom;
        private readonly HLTransciever marToAddressBus;
        private readonly SingleOrGate pc_leHi_leLo_to_le;
        private readonly SingleOrGate mar_leHi_leLo_to_le;

        private Signal mar_oe_const;

        public BitArray PcContent => pc.Content;
        public BitArray MarContent => mar.Content;

        public Memory(BitArray[] romData, BitArray[] ramData, Clock mainClock, IBus dataBus, IBus addressBus) {
            pc = new HLHiLoCounter(16);
            mar = new HLHiLoCounter(16);
            rom = new HLRom(8, 16, MemorySize, romData);
            ram = new HLRam(8, 16, MemorySize, ramData);
            marToAddressBus = new HLTransciever(16);
            pc_leHi_leLo_to_le = new SingleOrGate(2);
            mar_leHi_leLo_to_le = new SingleOrGate(2);

            ConnectInternals();
            CreateAndSetConstSignals();
            ConnectMainClock(mainClock);
            ConnectDataBus(dataBus);
            ConnectAddressBus(addressBus);
        }

        protected override void ConnectMainClock(Clock mainClock) {
            pc.Clk.PlugIn(mainClock.Clk);
            mar.Clk.PlugIn(mainClock.Clk);
            ram.Clk.PlugIn(mainClock.Clk);
        }

        protected override void ConnectInternals() {
            Signal.Factory.CreateAndConnectPorts(nameof(marToAddressBus), mar.Outputs, marToAddressBus.Inputs);
            Signal.Factory.CreateAndConnectPorts("MarToRom", mar.Outputs, rom.AddressInputs);
            Signal.Factory.CreateAndConnectPorts("MarToRam", mar.Outputs, ram.AddressInputs);

            Signal.Factory.CreateAndConnectPort(nameof(pc_leHi_leLo_to_le), pc_leHi_leLo_to_le.Output, pc.LoadEnable);
            Signal.Factory.CreateAndConnectPort(nameof(mar_leHi_leLo_to_le), mar_leHi_leLo_to_le.Output, mar.LoadEnable);
        }

        protected override void CreateAndSetConstSignals() {
            (mar_oe_const = mar.CreateSignalAndPlugin(nameof(mar_oe_const), x => x.OutputEnable)).Value = true;
        }

        protected override void ConnectDataBus(IBus dataBus) {
            dataBus
                .Connect(0, 8, pc.Inputs.Take(8))
                .Connect(0, 8, pc.Inputs.TakeLast(8))

                .Connect(0, 8, mar.Inputs.Take(8))
                .Connect(0, 8, mar.Inputs.TakeLast(8))

                .Connect(0, 8, ram.DataInputs)
                .Connect(0, 8, ram.Outputs)

                .Connect(0, 8, rom.Outputs);
        }

        protected override void ConnectAddressBus(IBus addressBus) {
            addressBus
                .Connect(0, 16, pc.Inputs)
                .Connect(0, 16, pc.Outputs)

                .Connect(0, 16, mar.Inputs)
                .Connect(0, 16, marToAddressBus.Outputs);
        }

        public override CsPanel.MemoryPanel CreateControlPanel(IBus controlBus) {
            pc.CreateSignalAndPlugin("slave_pc.LoadEnableHigh", x => x.LoadEnableHigh)
                .SetMaster(controlBus.GetControlSignal(ControlSignalType.Pc_le_hi));

            pc.CreateSignalAndPlugin("slave_pc.LoadEnableLow", x => x.LoadEnableLow)
                .SetMaster(controlBus.GetControlSignal(ControlSignalType.Pc_le_lo));

            mar.CreateSignalAndPlugin("slave_mar.LoadEnableHigh", x => x.LoadEnableHigh)
                .SetMaster(controlBus.GetControlSignal(ControlSignalType.Mar_le_hi));

            mar.CreateSignalAndPlugin("slave_mar.LoadEnableLow", x => x.LoadEnableLow)
                .SetMaster(controlBus.GetControlSignal(ControlSignalType.Mar_le_lo));

            return new CsPanel.MemoryPanel {
                Pc_le_hi = controlBus.ConnectAsControlSignal(ControlSignalType.Pc_le_hi, pc_leHi_leLo_to_le.Inputs[0]),
                Pc_le_lo = controlBus.ConnectAsControlSignal(ControlSignalType.Pc_le_lo, pc_leHi_leLo_to_le.Inputs[1]),
                Pc_oe = controlBus.ConnectAsControlSignal(ControlSignalType.Pc_oe, pc.OutputEnable),
                Pc_ce = controlBus.ConnectAsControlSignal(ControlSignalType.Pc_ce, pc.CountEnable),

                Mar_le_hi = controlBus.ConnectAsControlSignal(ControlSignalType.Mar_le_hi, mar_leHi_leLo_to_le.Inputs[0]),
                Mar_le_lo = controlBus.ConnectAsControlSignal(ControlSignalType.Mar_le_lo, mar_leHi_leLo_to_le.Inputs[1]),
                MarToBus_oe = controlBus.ConnectAsControlSignal(ControlSignalType.MarToBus_oe, marToAddressBus.OutputEnable),
                Mar_ce = controlBus.ConnectAsControlSignal(ControlSignalType.Mar_ce, mar.CountEnable),

                Ram_we = controlBus.ConnectAsControlSignal(ControlSignalType.Ram_we, ram.WriteEnable),
                Ram_oe = controlBus.ConnectAsControlSignal(ControlSignalType.Ram_oe, ram.OutputEnable),

                Rom_oe = controlBus.ConnectAsControlSignal(ControlSignalType.Rom_oe, rom.OutputEnable),
            };
        }
    }
}

using _Infrastructure.BitArrays;
using Components.Buses;
using Components.Clocks;
using Components.Counters;
using Components.Logic;
using Components.Rams;
using Components.Roms;
using Components.Signals;
using Components.Transcievers;
using KPC8.ControlSignals;
using System.Collections;
using System.Linq;

namespace KPC8.Modules {
    public class Memory : ModuleBase {
        private const int MemorySize = 65536; // 2 ^ 16

        private readonly HLHiLoCounter pc;
        private readonly HLHiLoCounter mar;
        private readonly HLRam ram;
        private readonly HLRom rom;
        private readonly HLTransciever marToAddressBus;
        private readonly SingleOrGate pc_leHi_leLo_to_le;
        private readonly SingleOrGate mar_leHi_leLo_to_le;

        private Signal mar_oe_const;

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

        private void ConnectMainClock(Clock mainClock) {
            pc.Clk.PlugIn(mainClock.Clk);
            mar.Clk.PlugIn(mainClock.Clk);
            ram.Clk.PlugIn(mainClock.Clk);
        }

        private void ConnectInternals() {
            Signal.Factory.CreateAndConnectPorts(nameof(marToAddressBus), mar.Outputs, marToAddressBus.Inputs);
            Signal.Factory.CreateAndConnectPorts("MarToRom", mar.Outputs, rom.AddressInputs);
            Signal.Factory.CreateAndConnectPorts("MarToRam", mar.Outputs, ram.AddressInputs);

            Signal.Factory.CreateAndConnectPort(nameof(pc_leHi_leLo_to_le), pc_leHi_leLo_to_le.Output, pc.LoadEnable);
            Signal.Factory.CreateAndConnectPorts(nameof(pc_leHi_leLo_to_le), new[] { pc.LoadEnableHigh, pc.LoadEnableLow }, pc_leHi_leLo_to_le.Inputs);

            Signal.Factory.CreateAndConnectPort(nameof(mar_leHi_leLo_to_le), mar_leHi_leLo_to_le.Output, mar.LoadEnable);
            Signal.Factory.CreateAndConnectPorts(nameof(mar_leHi_leLo_to_le), new[] { mar.LoadEnableHigh, mar.LoadEnableLow }, mar_leHi_leLo_to_le.Inputs);
        }

        private void CreateAndSetConstSignals() {
            mar_oe_const = mar.CreateSignalAndPlugin(nameof(mar_oe_const), x => x.OutputEnable);
            mar_oe_const.Value = true;
        }

        private void ConnectDataBus(IBus dataBus) {
            dataBus
                .Connect(0, 8, pc.Inputs.Take(8))
                .Connect(0, 8, pc.Inputs.TakeLast(8))

                .Connect(0, 8, mar.Inputs.Take(8))
                .Connect(0, 8, mar.Inputs.TakeLast(8))

                .Connect(0, 8, ram.DataInputs)
                .Connect(0, 8, ram.Outputs)

                .Connect(0, 8, rom.Outputs);
        }

        private void ConnectAddressBus(IBus addressBus) {
            addressBus
                .Connect(0, 16, pc.Inputs)
                .Connect(0, 16, pc.Outputs)

                .Connect(0, 16, mar.Inputs)
                .Connect(0, 16, marToAddressBus.Outputs);
        }

        public CsPanel.MemoryPanel CreateMemoryPanel() {
            return new CsPanel.MemoryPanel {
                Pc_le_hi = pc_leHi_leLo_to_le.CreateSignalAndPlugin(nameof(CsPanel.MemoryPanel.Pc_le_hi), x => x.Inputs[0]),
                Pc_le_lo = pc_leHi_leLo_to_le.CreateSignalAndPlugin(nameof(CsPanel.MemoryPanel.Pc_le_lo), x => x.Inputs[1]),
                Pc_oe = pc.CreateSignalAndPlugin(nameof(CsPanel.MemoryPanel.Pc_oe), x => x.OutputEnable),
                Pc_ce = pc.CreateSignalAndPlugin(nameof(CsPanel.MemoryPanel.Pc_ce), x => x.CountEnable),

                Mar_le_hi = mar_leHi_leLo_to_le.CreateSignalAndPlugin(nameof(CsPanel.MemoryPanel.Mar_le_hi), x => x.Inputs[0]),
                Mar_le_lo = mar_leHi_leLo_to_le.CreateSignalAndPlugin(nameof(CsPanel.MemoryPanel.Mar_le_lo), x => x.Inputs[1]),
                MarToBus_oe = marToAddressBus.CreateSignalAndPlugin(nameof(CsPanel.MemoryPanel.MarToBus_oe), x => x.OutputEnable),
                Mar_ce = mar.CreateSignalAndPlugin(nameof(CsPanel.MemoryPanel.Mar_ce), x => x.CountEnable),

                Ram_we = ram.CreateSignalAndPlugin(nameof(CsPanel.MemoryPanel.Ram_we), x => x.WriteEnable),
                Ram_oe = ram.CreateSignalAndPlugin(nameof(CsPanel.MemoryPanel.Ram_oe), x => x.OutputEnable),

                Rom_oe = rom.CreateSignalAndPlugin(nameof(CsPanel.MemoryPanel.Rom_oe), x => x.OutputEnable),
            };
        }
    }
}

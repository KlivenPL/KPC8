using _Infrastructure.BitArrays;
using Components.Buses;
using Components.Clocks;
using Components.Counters;
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

        private Signal mar_oe_const;

        public Memory(BitArray[] romData, Clock mainClock, IBus dataBus, IBus addressBus) {
            pc = new HLHiLoCounter(16);
            mar = new HLHiLoCounter(16);
            ram = new HLRam(8, 16, MemorySize);
            rom = new HLRom(8, 16, MemorySize, romData);
            marToAddressBus = new HLTransciever(16);

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
                Pc_le_hi = pc.CreateSignalAndPlugin(nameof(CsPanel.MemoryPanel.Pc_le_hi), x => x.LoadEnableHigh),
                Pc_le_lo = pc.CreateSignalAndPlugin(nameof(CsPanel.MemoryPanel.Pc_le_lo), x => x.LoadEnableLow),
                Pc_le = pc.CreateSignalAndPlugin(nameof(CsPanel.MemoryPanel.Pc_le), x => x.LoadEnable),
                Pc_oe = pc.CreateSignalAndPlugin(nameof(CsPanel.MemoryPanel.Pc_oe), x => x.OutputEnable),
                Pc_ce = pc.CreateSignalAndPlugin(nameof(CsPanel.MemoryPanel.Pc_ce), x => x.CountEnable),

                Mar_le_hi = mar.CreateSignalAndPlugin(nameof(CsPanel.MemoryPanel.Mar_le_hi), x => x.LoadEnableHigh),
                Mar_le_lo = mar.CreateSignalAndPlugin(nameof(CsPanel.MemoryPanel.Mar_le_lo), x => x.LoadEnableLow),
                Mar_le = mar.CreateSignalAndPlugin(nameof(CsPanel.MemoryPanel.Mar_le), x => x.LoadEnable),
                MarToBus_oe = marToAddressBus.CreateSignalAndPlugin(nameof(CsPanel.MemoryPanel.MarToBus_oe), x => x.OutputEnable),
                Mar_ce = mar.CreateSignalAndPlugin(nameof(CsPanel.MemoryPanel.Mar_ce), x => x.CountEnable),

                Ram_we = ram.CreateSignalAndPlugin(nameof(CsPanel.MemoryPanel.Ram_we), x => x.WriteEnable),
                Ram_oe = ram.CreateSignalAndPlugin(nameof(CsPanel.MemoryPanel.Ram_oe), x => x.OutputEnable),

                Rom_oe = rom.CreateSignalAndPlugin(nameof(CsPanel.MemoryPanel.Rom_oe), x => x.OutputEnable),
            };
        }
    }
}

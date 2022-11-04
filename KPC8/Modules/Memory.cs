using _Infrastructure.BitArrays;
using Components.Buses;
using Components.Counters;
using Components.Logic;
using Components.Multiplexers;
using Components.Rams;
using Components.Roms;
using Components.Signals;
using Components.Transcievers;
using Infrastructure.BitArrays;
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

        private readonly HLSingleSwitch2NToNMux busSelectPc;
        private readonly HLSingleSwitch2NToNMux busSelectMar;
        private readonly SingleAndGate pc_leHi_and_leLo_to_addressBusSelect;
        private readonly SingleAndGate mar_leHi_and_leLo_to_addressBusSelect;

        private readonly HLHiLoTransciever addrBusToDataBus;

        private Signal mar_oe_const;
        public BitArray PcContent => pc.Content;
        public BitArray MarContent => mar.Content;

        public BitArray GetRamAt(ushort address) => ram.Content[address];
        public BitArray GetRomAt(ushort address) => rom.Content[address];

        public byte[] RamDumpToBytesLE() {
            var bytes = new byte[MemorySize];

            for (int i = 0; i < MemorySize; i++) {
                bytes[i] = BitArrayHelper.ToByteLE(ram.Content[i]);
            }

            return bytes;
        }

        public byte[] RomDumpToBytesLE() {
            var bytes = new byte[MemorySize];

            for (int i = 0; i < MemorySize; i++) {
                bytes[i] = BitArrayHelper.ToByteLE(rom.Content[i]);
            }

            return bytes;
        }

        public Memory(BitArray[] romData, BitArray[] ramData, Signal mainClock, IBus dataBus, IBus addressBus) {
            pc = new HLHiLoCounter(nameof(pc), 16);
            mar = new HLHiLoCounter(nameof(mar), 16);
            rom = new HLRom(nameof(rom), 8, 16, MemorySize, romData);
            ram = new HLRam(nameof(ram), 8, 16, MemorySize, ramData);
            marToAddressBus = new HLTransciever(nameof(marToAddressBus), 16);
            busSelectPc = new HLSingleSwitch2NToNMux(nameof(busSelectPc), 16);
            busSelectMar = new HLSingleSwitch2NToNMux(nameof(busSelectMar), 16);
            pc_leHi_and_leLo_to_addressBusSelect = new SingleAndGate(nameof(pc_leHi_and_leLo_to_addressBusSelect), 2);
            mar_leHi_and_leLo_to_addressBusSelect = new SingleAndGate(nameof(mar_leHi_and_leLo_to_addressBusSelect), 2);
            addrBusToDataBus = new HLHiLoTransciever(nameof(addrBusToDataBus), 16);

            ConnectInternals();
            CreateAndSetConstSignals();
            ConnectMainClock(mainClock);
            ConnectDataBus(dataBus);
            ConnectAddressBus(addressBus);
        }

        protected override void ConnectMainClock(Signal mainClock) {
            pc.Clk.PlugIn(mainClock);
            mar.Clk.PlugIn(mainClock);
            ram.Clk.PlugIn(mainClock);
            busSelectPc.Clk.PlugIn(mainClock);
            busSelectMar.Clk.PlugIn(mainClock);
        }

        protected override void ConnectInternals() {
            Signal.Factory.CreateAndConnectPorts(nameof(marToAddressBus), mar.Outputs, marToAddressBus.Inputs);
            Signal.Factory.CreateAndConnectPorts("MarToRom", mar.Outputs, rom.AddressInputs);
            Signal.Factory.CreateAndConnectPorts("MarToRam", mar.Outputs, ram.AddressInputs);

            Signal.Factory.CreateAndConnectPort(nameof(pc_leHi_and_leLo_to_addressBusSelect), pc_leHi_and_leLo_to_addressBusSelect.Output, busSelectPc.SelectB);
            Signal.Factory.CreateAndConnectPort(nameof(mar_leHi_and_leLo_to_addressBusSelect), mar_leHi_and_leLo_to_addressBusSelect.Output, busSelectMar.SelectB);

            Signal.Factory.CreateAndConnectPorts(nameof(busSelectPc), busSelectPc.Outputs, pc.Inputs);
            Signal.Factory.CreateAndConnectPorts(nameof(busSelectMar), busSelectMar.Outputs, mar.Inputs);
        }

        protected override void CreateAndSetConstSignals() {
            (mar_oe_const = mar.CreateSignalAndPlugin(nameof(mar_oe_const), x => x.OutputEnable)).Value = true;
        }

        protected override void ConnectDataBus(IBus dataBus) {
            dataBus
                //.Connect(0, 8, pc.Inputs.Take(8))
                //.Connect(0, 8, pc.Inputs.TakeLast(8))
                .Connect(0, 8, busSelectPc.InputsA.Take(8))
                .Connect(0, 8, busSelectPc.InputsA.TakeLast(8))

                //.Connect(0, 8, mar.Inputs.Take(8))
                //.Connect(0, 8, mar.Inputs.TakeLast(8))
                .Connect(0, 8, busSelectMar.InputsA.Take(8))
                .Connect(0, 8, busSelectMar.InputsA.TakeLast(8))

                .Connect(0, 8, ram.DataInputs)
                .Connect(0, 8, ram.Outputs)

                .Connect(0, 8, rom.Outputs)

                .Connect(0, 8, addrBusToDataBus.Outputs.Take(8))
                .Connect(0, 8, addrBusToDataBus.Outputs.TakeLast(8));
        }

        protected override void ConnectAddressBus(IBus addressBus) {
            addressBus
                //.Connect(0, 16, pc.Inputs)
                .Connect(0, 16, busSelectPc.InputsB)
                .Connect(0, 16, pc.Outputs)

                //.Connect(0, 16, mar.Inputs)
                .Connect(0, 16, busSelectMar.InputsB)
                .Connect(0, 16, marToAddressBus.Outputs)

                .Connect(0, 16, addrBusToDataBus.Inputs);
        }

        public override CsPanel.MemoryPanel CreateControlPanel(IBus controlBus) {
            pc.LoadEnableHigh.PlugIn(controlBus.GetControlSignal(ControlSignalType.Pc_le_hi));
            pc.LoadEnableLow.PlugIn(controlBus.GetControlSignal(ControlSignalType.Pc_le_lo));

            mar.LoadEnableHigh.PlugIn(controlBus.GetControlSignal(ControlSignalType.Mar_le_hi));
            mar.LoadEnableLow.PlugIn(controlBus.GetControlSignal(ControlSignalType.Mar_le_lo));

            pc_leHi_and_leLo_to_addressBusSelect.Inputs[0].PlugIn(controlBus.GetControlSignal(ControlSignalType.Pc_le_hi));
            pc_leHi_and_leLo_to_addressBusSelect.Inputs[1].PlugIn(controlBus.GetControlSignal(ControlSignalType.Pc_le_lo));

            mar_leHi_and_leLo_to_addressBusSelect.Inputs[0].PlugIn(controlBus.GetControlSignal(ControlSignalType.Mar_le_hi));
            mar_leHi_and_leLo_to_addressBusSelect.Inputs[1].PlugIn(controlBus.GetControlSignal(ControlSignalType.Mar_le_lo));

            return new CsPanel.MemoryPanel {
                Pc_le_hi = controlBus.ConnectAsControlSignal(ControlSignalType.Pc_le_hi, pc.LoadEnable),
                Pc_le_lo = controlBus.ConnectAsControlSignal(ControlSignalType.Pc_le_lo, pc.LoadEnable),
                Pc_oe = controlBus.ConnectAsControlSignal(ControlSignalType.Pc_oe, pc.OutputEnable),
                Pc_ce = controlBus.ConnectAsControlSignal(ControlSignalType.Pc_ce, pc.CountEnable),

                Mar_le_hi = controlBus.ConnectAsControlSignal(ControlSignalType.Mar_le_hi, mar.LoadEnable),
                Mar_le_lo = controlBus.ConnectAsControlSignal(ControlSignalType.Mar_le_lo, mar.LoadEnable),
                MarToBus_oe = controlBus.ConnectAsControlSignal(ControlSignalType.MarToBus_oe, marToAddressBus.OutputEnable),
                Mar_ce = controlBus.ConnectAsControlSignal(ControlSignalType.Mar_ce, mar.CountEnable),

                Ram_we = controlBus.ConnectAsControlSignal(ControlSignalType.Ram_we, ram.WriteEnable),
                Ram_oe = controlBus.ConnectAsControlSignal(ControlSignalType.Ram_oe, ram.OutputEnable),

                Rom_oe = controlBus.ConnectAsControlSignal(ControlSignalType.Rom_oe, rom.OutputEnable),

                AddrToData_hi = controlBus.ConnectAsControlSignal(ControlSignalType.AddrToData_hi, addrBusToDataBus.OutputEnableHi),
                AddrToData_lo = controlBus.ConnectAsControlSignal(ControlSignalType.AddrToData_lo, addrBusToDataBus.OutputEnableLo),
            };
        }
    }
}

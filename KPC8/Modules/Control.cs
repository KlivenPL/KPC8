using _Infrastructure.BitArrays;
using Components.Buses;
using Components.Clocks;
using Components.Counters;
using Components.Decoders;
using Components.Logic;
using Components.Registers;
using Components.Roms;
using Components.Signals;
using Components.Transcievers;
using KPC8._Infrastructure.Components;
using KPC8.ControlSignals;
using System.Collections;
using System.Linq;

namespace KPC8.Modules {
    public class Control : ModuleBase<CsPanel.ControlPanel> {
        private const int InstRomSize = 1024;
        private const int OpCodeLength = 6;

        private const int DestRegEncodedLength = 2;
        private const int ARegEncodedSize = 4;
        private const int BRegEncodedSize = 4;

        private readonly HLHiLoRegister ir;
        private readonly HLCounter ic;
        private readonly HLRom instRom;
        private readonly HLDecoder decDest;
        private readonly HLDecoder decA;
        private readonly HLDecoder decB;
        private readonly HLTransciever ir8LsbToBus;
        private readonly SingleOrGate ir_leHi_leLo_to_le;

        private Signal ir_oe_const;
        private Signal ic_ce_const;

        public BitArray IrOutput => ir.Outputs.ToBitArray();
        public BitArray IcOutput => ic.Outputs.ToBitArray();
        public BitArray DecDestOutput => decDest.Outputs.ToBitArray();
        public BitArray DecAOutput => decA.Outputs.ToBitArray();
        public BitArray DecBOutput => decB.Outputs.ToBitArray();

        public Control(BitArray[] instrData, Clock mainClockBar, IBus dataBus, IBus registerSelectBus) {
            ir = new HLHiLoRegister(16);
            ic = new HLCounter(4);
            instRom = new HLRom(32, 10, InstRomSize, instrData);
            decDest = new HLDecoder(DestRegEncodedLength);
            decA = new HLDecoder(ARegEncodedSize);
            decB = new HLDecoder(BRegEncodedSize);
            ir8LsbToBus = new HLTransciever(8);
            ir_leHi_leLo_to_le = new SingleOrGate(2);

            ConnectInternals();
            CreateAndSetConstSignals();
            ConnectMainClockBar(mainClockBar);
            ConnectDataBus(dataBus);
            ConnectRegisterSelectBus(registerSelectBus);
        }

        protected override void ConnectMainClockBar(Clock mainClockBar) {
            ir.Clk.PlugIn(mainClockBar.Clk);
            ic.Clk.PlugIn(mainClockBar.Clk);
        }

        protected override void ConnectInternals() {
            Signal.Factory.CreateAndConnectPorts(nameof(ir8LsbToBus), ir.Outputs.TakeLast(8), ir8LsbToBus.Inputs);

            Signal.Factory.CreateAndConnectPorts("IrOpCodeToInstRom",
                ir.Outputs
                    .Take(OpCodeLength),
                instRom.AddressInputs
                    .Take(OpCodeLength));

            Signal.Factory.CreateAndConnectPorts("IrToDecDest",
                ir.Outputs
                    .Skip(OpCodeLength)
                    .Take(DestRegEncodedLength),
                decDest.Inputs);

            Signal.Factory.CreateAndConnectPorts("IrToDecA",
                ir.Outputs
                    .Skip(OpCodeLength + DestRegEncodedLength)
                    .Take(ARegEncodedSize),
                decA.Inputs);

            Signal.Factory.CreateAndConnectPorts("IrToDecB",
                ir.Outputs
                    .Skip(OpCodeLength + DestRegEncodedLength + ARegEncodedSize)
                    .Take(BRegEncodedSize),
                decB.Inputs);

            Signal.Factory.CreateAndConnectPorts("IcToInstRom", ic.Outputs, instRom.AddressInputs.Skip(OpCodeLength));

            Signal.Factory.CreateAndConnectPort(nameof(ir_leHi_leLo_to_le), ir_leHi_leLo_to_le.Output, ir.LoadEnable);
        }

        protected override void CreateAndSetConstSignals() {
            (ir_oe_const = ir.CreateSignalAndPlugin(nameof(ir_oe_const), x => x.OutputEnable)).Value = true;
            (ic_ce_const = ic.CreateSignalAndPlugin(nameof(ic_ce_const), x => x.CountEnable)).Value = true;
        }

        protected override void ConnectDataBus(IBus dataBus) {
            dataBus
                .Connect(0, 8, ir8LsbToBus.Outputs)
                .Connect(0, 8, ir.Inputs.Take(8))
                .Connect(0, 8, ir.Inputs.TakeLast(8));
        }

        protected override void ConnectRegisterSelectBus(IBus registerSelectBus) {
            registerSelectBus
                .Connect(4, 4, decDest.Outputs)
                .Connect(0, 16, decA.Outputs)
                .Connect(0, 16, decB.Outputs);
        }

        public override CsPanel.ControlPanel CreateControlPanel(IBus controlBus) {
            ConnectControlBusToControllerPorts(controlBus);

            ir.LoadEnableHigh.PlugIn(controlBus.GetControlSignal(ControlSignalType.Ir_le_hi));
            ir.LoadEnableLow.PlugIn(controlBus.GetControlSignal(ControlSignalType.Ir_le_lo));

            return new CsPanel.ControlPanel {
                Ir_le_hi = controlBus.ConnectAsControlSignal(ControlSignalType.Ir_le_hi, ir_leHi_leLo_to_le.Inputs[0]),
                Ir_le_lo = controlBus.ConnectAsControlSignal(ControlSignalType.Ir_le_lo, ir_leHi_leLo_to_le.Inputs[1]),
                Ir8LSBToBus_oe = controlBus.ConnectAsControlSignal(ControlSignalType.Ir8LSBToBus_oe, ir8LsbToBus.OutputEnable),

                Ic_clr = controlBus.ConnectAsControlSignal(ControlSignalType.Ic_clr, ic.Clear),

                DecDest_oe = controlBus.ConnectAsControlSignal(ControlSignalType.DecDest_oe, decDest.OutputEnable),
                DecA_oe = controlBus.ConnectAsControlSignal(ControlSignalType.DecA_oe, decA.OutputEnable),
                DecB_oe = controlBus.ConnectAsControlSignal(ControlSignalType.DecB_oe, decB.OutputEnable),
            };
        }

        private void ConnectControlBusToControllerPorts(IBus controlBus) {
            controlBus.ConnectToControllerPort(ControlSignalType.Ir_le_hi, instRom.Outputs);
            controlBus.ConnectToControllerPort(ControlSignalType.Ir_le_lo, instRom.Outputs);
            controlBus.ConnectToControllerPort(ControlSignalType.Ir8LSBToBus_oe, instRom.Outputs);

            controlBus.ConnectToControllerPort(ControlSignalType.Ic_clr, instRom.Outputs);

            controlBus.ConnectToControllerPort(ControlSignalType.DecDest_oe, instRom.Outputs);
            controlBus.ConnectToControllerPort(ControlSignalType.DecA_oe, instRom.Outputs);
            controlBus.ConnectToControllerPort(ControlSignalType.DecB_oe, instRom.Outputs);
        }
    }
}

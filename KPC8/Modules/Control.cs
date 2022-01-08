using _Infrastructure.BitArrays;
using Components.Buses;
using Components.Counters;
using Components.Decoders;
using Components.Logic;
using Components.Multiplexers;
using Components.Registers;
using Components.Roms;
using Components.Sequencers;
using Components.Signals;
using Components.Transcievers;
using Infrastructure.BitArrays;
using KPC8._Infrastructure.Components;
using KPC8.ControlSignals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KPC8.Modules {
    public class Control : ModuleBase<CsPanel.ControlPanel> {
        private const int InstRomSize = 2048;
        private const int IrrRomSize = 16;
        private const int OpCodeLength = 6;
        private const int CondOpCodeLength = 3;
        private const int FlagBusLength = 4;

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
        private readonly HLSingleSwitch2NToNMux instructionSelectMux;
        private readonly HLSingleSwitch2NToNMux irBusSelectMux;

        private readonly HLIRRSequencer irr;
        private readonly HLRom irrRom;

        private readonly SingleOrGate ir_leHi_leLo_to_le;
        private readonly SingleAndGate condInstructionDetector;

        private Signal ir_oe_const;
        private Signal ic_ce_const;
        private Signal ic_oe_const;
        private Signal instRom_oe_const;
        private Signal irrRom_oe_const;
        private Signal decDest_Input1_const;

        public BitArray IrOutput => ir.Outputs.ToBitArray();
        public BitArray IrContent => ir.Content;
        public BitArray IcOutput => ic.Outputs.ToBitArray();
        public BitArray DecDestInput => decDest.Inputs.ToBitArray();
        public BitArray DecDestOutput => decDest.Outputs.ToBitArray();
        public BitArray DecAInput => decA.Inputs.ToBitArray();
        public BitArray DecAOutput => decA.Outputs.ToBitArray();
        public BitArray DecBInput => decB.Inputs.ToBitArray();
        public BitArray DecBOutput => decB.Outputs.ToBitArray();
        public BitArray InstRomAddress => instRom.AddressInputs.ToBitArray();
        public BitArray IrrCode => irr.IrrCode;
        public bool GetIrrSignal(Func<HLIRRSequencer, SignalPort> selector) => selector(irr);
        public BitArray IrrRomAddress => irrRom.AddressInputs.ToBitArray();
        public BitArray GetIrrExpectedRomData(int row) => GetIrrRomData().ToArray()[row];

        public Control(BitArray[] instrData, Signal mainClockBar, IBus dataBus, IBus registerSelectBus, IBus flagsBus, IBus interruptsBus) {
            ir = new HLHiLoRegister(nameof(ir), 16);
            ic = new HLCounter(nameof(ic), 4);
            instRom = new HLRom(nameof(instRom), 32, 11, InstRomSize, instrData);
            decDest = new HLDecoder(nameof(decDest), DestRegEncodedLength + 2);
            decA = new HLDecoder(nameof(decA), ARegEncodedSize);
            decB = new HLDecoder(nameof(decB), BRegEncodedSize);
            ir8LsbToBus = new HLTransciever(nameof(ir8LsbToBus), 8);
            instructionSelectMux = new HLSingleSwitch2NToNMux(nameof(instructionSelectMux), 10);
            ir_leHi_leLo_to_le = new SingleOrGate(nameof(ir_leHi_leLo_to_le), 2);
            condInstructionDetector = new SingleAndGate(nameof(condInstructionDetector), 3);

            irr = new HLIRRSequencer(nameof(irr));
            irrRom = new HLRom(nameof(irrRom), 16, 4, IrrRomSize, GetIrrRomData().ToArray());
            irBusSelectMux = new HLSingleSwitch2NToNMux(nameof(irBusSelectMux), 16);

            ConnectInternals();
            CreateAndSetConstSignals();
            ConnectMainClockBar(mainClockBar);
            ConnectDataBus(dataBus);
            ConnectRegisterSelectBus(registerSelectBus);
            ConnectFlagsBus(flagsBus);
            ConnectInterruptsBus(interruptsBus);
        }

        private IEnumerable<BitArray> GetIrrRomData() {
            yield return BitArrayHelper.FromString("1101000011110000");
            yield return BitArrayHelper.FromString("1101000011100000");
            yield return BitArrayHelper.FromString("1101000011010000");
            yield return BitArrayHelper.FromString("1101000011000000");
            yield return BitArrayHelper.FromString("1101000010110000");
            yield return BitArrayHelper.FromString("1101000010100000");
            yield return BitArrayHelper.FromString("1101000010010000");
            yield return BitArrayHelper.FromString("1101000010000000");
            yield return BitArrayHelper.FromString("1101000001110000");
            yield return BitArrayHelper.FromString("1101000001100000");
            yield return BitArrayHelper.FromString("1101000001010000");
            yield return BitArrayHelper.FromString("1101000001000000");
            yield return BitArrayHelper.FromString("1101000000110000");
            yield return BitArrayHelper.FromString("1101000000100000");
            yield return BitArrayHelper.FromString("1101000000010000");
            yield return BitArrayHelper.FromString("1101000000000000");
        }

        protected override void ConnectMainClockBar(Signal mainClockBar) {
            ir.Clk.PlugIn(mainClockBar);
            ic.Clk.PlugIn(mainClockBar);
            irr.MainClock.PlugIn(mainClockBar);
        }

        protected override void ConnectInternals() {
            Signal.Factory.CreateAndConnectPorts(nameof(ir8LsbToBus), ir.Outputs.TakeLast(8), ir8LsbToBus.Inputs);

            Signal.Factory.CreateAndConnectPorts("IrOpCodeToInstrMuxProcedural",
                ir.Outputs
                    .Take(OpCodeLength),
                instructionSelectMux.InputsA
                    .Take(6));

            Signal.Factory.CreateAndConnectPorts("IrOpCodeToInstrMuxConditional",
                ir.Outputs
                    .Skip(OpCodeLength - CondOpCodeLength)
                    .Take(CondOpCodeLength),
                instructionSelectMux.InputsB
                    .Take(3));

            Signal.Factory.CreateAndConnectPorts("IrToDecDest",
                ir.Outputs
                    .Skip(OpCodeLength)
                    .Take(DestRegEncodedLength),
                decDest.Inputs
                    .Skip(2)
                    .Take(2));

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

            Signal.Factory.CreateAndConnectPorts("IcToMuxProcedural",
                ic.Outputs,
                instructionSelectMux.InputsA
                    .Skip(OpCodeLength));

            Signal.Factory.CreateAndConnectPorts("IcToMuxConditional",
                ic.Outputs
                    .Skip(1),
                instructionSelectMux.InputsB
                    .Skip(CondOpCodeLength + FlagBusLength));

            Signal.Factory.CreateAndConnectPort(nameof(ir_leHi_leLo_to_le), ir_leHi_leLo_to_le.Output, ir.LoadEnable);

            Signal.Factory.CreateAndConnectPorts("MuxToInstRom", instructionSelectMux.Outputs, instRom.AddressInputs.Skip(1));

            Signal.Factory.CreateAndConnectPorts("CondInstructionDetectorInputs",
                condInstructionDetector.Inputs,
                ir.Outputs
                    .Take(CondOpCodeLength));

            var condInstructionDetectorOutput = Signal.Factory.CreateAndConnectPort("CondInstructionDetectorOutputs", condInstructionDetector.Output, instructionSelectMux.SelectB);
            instRom.AddressInputs[0].PlugIn(condInstructionDetectorOutput);

            Signal.Factory.CreateAndConnectPorts("IrBusSelectToIr", ir.Inputs, irBusSelectMux.Outputs);
            Signal.Factory.CreateAndConnectPorts("IRRRomToIrBusSelect", irrRom.Outputs, irBusSelectMux.InputsB);
            Signal.Factory.CreateAndConnectPorts("IRRCodeToIrrRomAddress", irr.Outputs, irrRom.AddressInputs);
            Signal.Factory.CreateAndConnectPort("IRRShouldProcessInterruptToSelectB", irr.ShouldProcessInterrupt, irBusSelectMux.SelectB);
        }

        protected override void CreateAndSetConstSignals() {
            (ir_oe_const = ir.CreateSignalAndPlugin(nameof(ir_oe_const), x => x.OutputEnable)).Value = true;
            (ic_ce_const = ic.CreateSignalAndPlugin(nameof(ic_ce_const), x => x.CountEnable)).Value = true;
            (ic_oe_const = ic.CreateSignalAndPlugin(nameof(ic_oe_const), x => x.OutputEnable)).Value = true;
            (decDest_Input1_const = decDest.CreateSignalAndPlugin(nameof(decDest_Input1_const), x => x.Inputs[1])).Value = true;
            (instRom_oe_const = instRom.CreateSignalAndPlugin(nameof(instRom_oe_const), x => x.OutputEnable)).Value = true;
            (irrRom_oe_const = irrRom.CreateSignalAndPlugin(nameof(irrRom_oe_const), x => x.OutputEnable)).Value = true;
        }

        protected override void ConnectDataBus(IBus dataBus) {
            dataBus
                .Connect(0, 8, ir8LsbToBus.Outputs)
                /*.Connect(0, 8, ir.Inputs.Take(8))
                .Connect(0, 8, ir.Inputs.TakeLast(8));*/
                .Connect(0, 8, irBusSelectMux.InputsA.Take(8))
                .Connect(0, 8, irBusSelectMux.InputsA.TakeLast(8));
        }

        protected override void ConnectRegisterSelectBus(IBus registerSelectBus) {
            registerSelectBus
                .Connect(0, 16, decDest.Outputs)
                .Connect(0, 16, decA.Outputs)
                .Connect(0, 16, decB.Outputs);
        }

        public override CsPanel.ControlPanel CreateControlPanel(IBus controlBus) {
            ir.LoadEnableHigh.PlugIn(controlBus.GetControlSignal(ControlSignalType.Ir_le_hi));
            ir.LoadEnableLow.PlugIn(controlBus.GetControlSignal(ControlSignalType.Ir_le_lo));

            irr.Ir_le_hi.PlugIn(controlBus.GetControlSignal(ControlSignalType.Ir_le_hi));
            irr.Ir_le_hi.PlugIn(controlBus.GetControlSignal(ControlSignalType.Ir_le_lo));

            return new CsPanel.ControlPanel {
                Ir_le_hi = controlBus.ConnectAsControlSignal(ControlSignalType.Ir_le_hi, ir_leHi_leLo_to_le.Inputs[0]),
                Ir_le_lo = controlBus.ConnectAsControlSignal(ControlSignalType.Ir_le_lo, ir_leHi_leLo_to_le.Inputs[1]),
                Ir8LSBToBus_oe = controlBus.ConnectAsControlSignal(ControlSignalType.Ir8LSBToBus_oe, ir8LsbToBus.OutputEnable),

                Ic_clr = controlBus.ConnectAsControlSignal(ControlSignalType.Ic_clr, ic.Clear),

                DecDest_oe = controlBus.ConnectAsControlSignal(ControlSignalType.DecDest_oe, decDest.OutputEnable),
                DecA_oe = controlBus.ConnectAsControlSignal(ControlSignalType.DecA_oe, decA.OutputEnable),
                DecB_oe = controlBus.ConnectAsControlSignal(ControlSignalType.DecB_oe, decB.OutputEnable),

                Irr_a = controlBus.ConnectAsControlSignal(ControlSignalType.Irr_a, irr.Irr_a),
                Irr_b = controlBus.ConnectAsControlSignal(ControlSignalType.Irr_b, irr.Irr_b),
            };
        }

        public void ConnectControlBusToControllerPorts(IBus controlBus) {
            foreach (var value in Enum.GetValues<ControlSignalType>().Skip(1)) {
                controlBus.ConnectToControllerPort(value, instRom.Outputs);
            }
        }

        protected override void ConnectFlagsBus(IBus flagsBus) {
            flagsBus.Connect(0, 4, instructionSelectMux.InputsB.Skip(CondOpCodeLength).Take(FlagBusLength));
        }

        protected override void ConnectInterruptsBus(IBus interruptsBus) {
            interruptsBus
                .Connect(0, irr.IRRRQ)
                .Connect(1, irr.RDY)
                .Connect(2, irr.EN)
                .Connect(3, irr.BUSY)
                .Connect(4, 4, irr.Inputs);
        }
    }
}

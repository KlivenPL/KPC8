using _Infrastructure.BitArrays;
using Components.Adders;
using Components.Buses;
using Components.Logic;
using Components.Multiplexers;
using Components.Registers;
using Components.Signals;
using Components.Transcievers;
using KPC8._Infrastructure.Components;
using KPC8.ControlSignals;
using System.Collections;
using System.Linq;

namespace KPC8.Modules {
    public class Alu : ModuleBase<CsPanel.AluPanel> {
        private readonly HLAdder adder;
        private readonly HLRegister regA;
        private readonly HLRegister regB;
        private readonly HLRegister regFlags;
        private readonly HLTransciever regAToDataBus;
        private readonly HLTransciever regBToDataBus;
        private readonly HLTransciever flagsBusToDataBus;
        private readonly SingleAndGate useCarryInOnModifier;
        private readonly SingleXorGate shouldFlagsBeUpdated;
        private readonly HLSingleSwitch2NToNMux selectDataBusToFlags;

        private Signal regA_oe_const;
        private Signal regB_oe_const;
        private Signal regFlags_oe_const;

        public BitArray RegAContent => regA.Content;
        public BitArray RegBContent => regB.Content;
        public BitArray AdderContent => adder.Content;
        public BitArray RegFlagsContent => regFlags.Content;
        public void SetRegFlagsContent(BitArray value) => regFlags.SetContent(value);

        public Alu(Signal mainClock, IBus dataBus, IBus flagsBus) {
            adder = new HLAdder("Adder", 8);
            regA = new HLRegister(nameof(regA), 8);
            regB = new HLRegister(nameof(regB), 8);
            shouldFlagsBeUpdated = new SingleXorGate("ShouldFlagsBeUpdated");
            selectDataBusToFlags = new HLSingleSwitch2NToNMux("SelectDataBusToFlags", 4);
            regFlags = new HLRegister(nameof(regFlags), 4);
            regAToDataBus = new HLTransciever(nameof(regAToDataBus), 8);
            regBToDataBus = new HLTransciever(nameof(regBToDataBus), 8);
            useCarryInOnModifier = new SingleAndGate(nameof(useCarryInOnModifier), 2);
            flagsBusToDataBus = new HLTransciever("FlagsBusToDataBus", 8);

            ConnectInternals();
            CreateAndSetConstSignals();
            ConnectMainClock(mainClock);
            ConnectDataBus(dataBus);
            ConnectFlagsBus(flagsBus);
        }

        protected override void ConnectMainClock(Signal mainClock) {
            regA.Clk.PlugIn(mainClock);
            regB.Clk.PlugIn(mainClock);
            regFlags.Clk.PlugIn(mainClock);
            selectDataBusToFlags.Clk.PlugIn(mainClock);
        }

        protected override void ConnectInternals() {
            Signal.Factory.CreateAndConnectPorts(nameof(regAToDataBus), regA.Outputs, regAToDataBus.Inputs);
            Signal.Factory.CreateAndConnectPorts(nameof(regBToDataBus), regB.Outputs, regBToDataBus.Inputs);

            Signal.Factory.CreateAndConnectPorts("RegAToAdder", regA.Outputs, adder.InputsA);
            Signal.Factory.CreateAndConnectPorts("RegBToAdder", regB.Outputs, adder.InputsB);

            Signal.Factory.CreateAndConnectPort("FlagsLe", shouldFlagsBeUpdated.Output, regFlags.LoadEnable);

            Signal.Factory.CreateAndConnectPort("ZfToRegFlags", adder.ZeroFlag, selectDataBusToFlags.InputsA[3]);
            Signal.Factory.CreateAndConnectPort("NfToRegFlags", adder.NegativeFlag, selectDataBusToFlags.InputsA[2]);
            Signal.Factory.CreateAndConnectPort("CfToRegFlags", adder.CarryFlag, selectDataBusToFlags.InputsA[1]);
            Signal.Factory.CreateAndConnectPort("OfToRegFlags", adder.OverflowFlag, selectDataBusToFlags.InputsA[0]);

            Signal.Factory.CreateAndConnectPorts(nameof(selectDataBusToFlags), selectDataBusToFlags.Outputs, regFlags.Inputs);

            Signal.Factory.CreateAndConnectPort("UseCarryInOnModifier_To_AdderCarryIn", useCarryInOnModifier.Output, adder.CarryIn);
        }

        protected override void CreateAndSetConstSignals() {
            (regA_oe_const = regA.CreateSignalAndPlugin(nameof(regA_oe_const), x => x.OutputEnable)).Value = true;
            (regB_oe_const = regB.CreateSignalAndPlugin(nameof(regB_oe_const), x => x.OutputEnable)).Value = true;
            (regFlags_oe_const = regFlags.CreateSignalAndPlugin(nameof(regFlags_oe_const), x => x.OutputEnable)).Value = true;
        }

        protected override void ConnectDataBus(IBus dataBus) {
            dataBus
                .Connect(0, 8, regA.Inputs)
                .Connect(0, 8, regAToDataBus.Outputs)

                .Connect(0, 8, regB.Inputs)
                .Connect(0, 8, regBToDataBus.Outputs)

                .Connect(0, 8, adder.Outputs)

                .Connect(0, 8, flagsBusToDataBus.Outputs)

                .Connect(4, selectDataBusToFlags.InputsB[0])
                .Connect(5, selectDataBusToFlags.InputsB[1])
                .Connect(6, selectDataBusToFlags.InputsB[2])
                .Connect(7, selectDataBusToFlags.InputsB[3]);
        }

        protected override void ConnectFlagsBus(IBus flagsBus) {
            flagsBus
                .Connect(0, 4, regFlags.Outputs)
                .Connect(1, useCarryInOnModifier.Inputs[0])
                .Connect(0, 4, flagsBusToDataBus.Inputs.TakeLast(4));
        }

        public override CsPanel.AluPanel CreateControlPanel(IBus controlBus) {
            shouldFlagsBeUpdated.Inputs[0].PlugIn(controlBus.GetControlSignal(ControlSignalType.Alu_oe));
            shouldFlagsBeUpdated.Inputs[1].PlugIn(controlBus.GetControlSignal(ControlSignalType.DataBusToFlags_le));

            var modifier = controlBus.GetControlSignal(ControlSignalType.MODIFIER);
            useCarryInOnModifier.Inputs[1].PlugIn(modifier);

            var ic_clr = controlBus.GetControlSignal(ControlSignalType.Ic_clr);
            regA.Clear.PlugIn(ic_clr);
            regB.Clear.PlugIn(ic_clr);

            return new CsPanel.AluPanel {
                RegAToBus_oe = controlBus.ConnectAsControlSignal(ControlSignalType.RegAToBus_oe, regAToDataBus.OutputEnable),
                RegBToBus_oe = controlBus.ConnectAsControlSignal(ControlSignalType.RegBToBus_oe, regBToDataBus.OutputEnable),
                FlagsToDataBus_oe = controlBus.ConnectAsControlSignal(ControlSignalType.FlagsToDataBus_oe, flagsBusToDataBus.OutputEnable),
                DataBusToFlags_le = controlBus.ConnectAsControlSignal(ControlSignalType.DataBusToFlags_le, selectDataBusToFlags.SelectB),
                RegA_le = controlBus.ConnectAsControlSignal(ControlSignalType.RegA_le, regA.LoadEnable),
                RegB_le = controlBus.ConnectAsControlSignal(ControlSignalType.RegB_le, regB.LoadEnable),
                Alu_oe = controlBus.ConnectAsControlSignal(ControlSignalType.Alu_oe, adder.OutputEnable),
                Alu_a = controlBus.ConnectAsControlSignal(ControlSignalType.Alu_a, adder.A),
                Alu_b = controlBus.ConnectAsControlSignal(ControlSignalType.Alu_b, adder.B),
                Alu_c = controlBus.ConnectAsControlSignal(ControlSignalType.Alu_c, adder.C),
            };
        }
    }
}

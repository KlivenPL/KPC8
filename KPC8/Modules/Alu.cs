using _Infrastructure.BitArrays;
using Components.Adders;
using Components.Buses;
using Components.Logic;
using Components.Registers;
using Components.Signals;
using Components.Transcievers;
using KPC8._Infrastructure.Components;
using KPC8.ControlSignals;
using System.Collections;

namespace KPC8.Modules {
    public class Alu : ModuleBase<CsPanel.AluPanel> {
        private readonly HLAdder adder;
        private readonly HLRegister regA;
        private readonly HLRegister regB;
        private readonly HLRegister regFlags;
        private readonly HLTransciever regAToDataBus;
        private readonly HLTransciever regBToDataBus;
        private readonly SingleAndGate useCarryInOnModifier;

        private Signal regA_oe_const;
        private Signal regB_oe_const;
        private Signal regFlags_oe_const;

        public BitArray RegAContent => regA.Content;
        public BitArray RegBContent => regB.Content;
        public BitArray AdderContent => adder.Content;
        public BitArray RegFlagsContent => regFlags.Content;

        public Alu(Signal mainClock, IBus dataBus, IBus flagsBus) {
            adder = new HLAdder("Adder", 8);
            regA = new HLRegister(nameof(regA), 8);
            regB = new HLRegister(nameof(regB), 8);
            regFlags = new HLRegister(nameof(regFlags), 4);
            regAToDataBus = new HLTransciever(nameof(regAToDataBus), 8);
            regBToDataBus = new HLTransciever(nameof(regBToDataBus), 8);
            useCarryInOnModifier = new SingleAndGate(nameof(useCarryInOnModifier), 2);

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
        }

        protected override void ConnectInternals() {
            Signal.Factory.CreateAndConnectPorts(nameof(regAToDataBus), regA.Outputs, regAToDataBus.Inputs);
            Signal.Factory.CreateAndConnectPorts(nameof(regBToDataBus), regB.Outputs, regBToDataBus.Inputs);

            Signal.Factory.CreateAndConnectPorts("RegAToAdder", regA.Outputs, adder.InputsA);
            Signal.Factory.CreateAndConnectPorts("RegBToAdder", regB.Outputs, adder.InputsB);

            Signal.Factory.CreateAndConnectPort("ZfToRegFlags", adder.ZeroFlag, regFlags.Inputs[3]);
            Signal.Factory.CreateAndConnectPort("NfToRegFlags", adder.NegativeFlag, regFlags.Inputs[2]);
            Signal.Factory.CreateAndConnectPort("CfToRegFlags", adder.CarryFlag, regFlags.Inputs[1]);
            Signal.Factory.CreateAndConnectPort("OfToRegFlags", adder.OverflowFlag, regFlags.Inputs[0]);

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

                .Connect(0, 8, adder.Outputs);
        }

        protected override void ConnectFlagsBus(IBus flagsBus) {
            flagsBus
                .Connect(0, 4, regFlags.Outputs)
                .Connect(1, useCarryInOnModifier.Inputs[0]);
        }

        public override CsPanel.AluPanel CreateControlPanel(IBus controlBus) {
            regFlags.LoadEnable.PlugIn(controlBus.GetControlSignal(ControlSignalType.Alu_oe));

            var modifier = controlBus.GetControlSignal(ControlSignalType.MODIFIER);
            useCarryInOnModifier.Inputs[1].PlugIn(modifier);

            var ic_clr = controlBus.GetControlSignal(ControlSignalType.Ic_clr);
            regA.Clear.PlugIn(ic_clr);
            regB.Clear.PlugIn(ic_clr);

            return new CsPanel.AluPanel {
                RegAToBus_oe = controlBus.ConnectAsControlSignal(ControlSignalType.RegAToBus_oe, regAToDataBus.OutputEnable),
                RegBToBus_oe = controlBus.ConnectAsControlSignal(ControlSignalType.RegBToBus_oe, regBToDataBus.OutputEnable),
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

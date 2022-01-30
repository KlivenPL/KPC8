using Components.Buses;
using Components.SignalMappers;
using Components.Signals;
using Components.Transcievers;
using ExternalDevices;
using ExternalDevices.HID;
using KPC8.ControlSignals;
using Simulation.Loops;
using System;

namespace KPC8.ExternalModules {
    public class KPadExternalModule : ExternalModuleBase {
        private readonly ushort address;
        private Signal kpad_cs_const;

        #region Run at main thread
        private readonly AddressMapper extInAddressMapper;
        private readonly HLTransciever kpadOutputToBusTransciever;
        #endregion

        #region Run at external thread
        private KPad kPad;
        #endregion

        private IExternalDevice KPadExternal => kPad;

        public KPadExternalModule(string name, ushort address) : base(name) {
            this.address = address;
            extInAddressMapper = AddressMapper.Create(16, mab => mab.Add(address));
            kpadOutputToBusTransciever = new HLTransciever("KPadOutput", 8);
        }

        public override void InitializeAndConnect(IBus dataBus, IBus addressBus, IBus interrputsBus, Signal extIn, Signal extOut) {
            extInAddressMapper.OutputEnable.PlugIn(extIn);
            kPad = new KPad(name);

            (kpad_cs_const = Signal.Factory.Create(nameof(kpad_cs_const))).Value = true;
            KPadExternal.ConnectAsExternalDevice(extIn, extOut, kpad_cs_const);

            ConnectAddressBus(addressBus);
            ConnectDataBus(dataBus);
            ConnectInternals();
        }

        public void SimulateButtons(KPadButtons buttons) {
            kPad.SimulatedButtons = buttons;
        }

        protected override void ConnectAddressBus(IBus addressBus) {
            addressBus
                .Connect(0, 16, extInAddressMapper.AddressInput);
        }

        protected override void ConnectDataBus(IBus dataBus) {
            dataBus
                .Connect(0, 8, kpadOutputToBusTransciever.Outputs);
        }

        protected override void ConnectInternals() {
            Signal.Factory.CreateAndConnectPorts("KpadToOutputTransciever", kPad.Outputs, kpadOutputToBusTransciever.Inputs);
           kpadOutputToBusTransciever.OutputEnable.PlugIn(extInAddressMapper.IsMatch);
        }
    }

    public static class KPadExternalModuleExtensions {
        public static CpuBuilder AddKPad(this CpuBuilder builder, string name, ushort address, out KPadExternalModule kPadModule, out Func<SimulationLoop> getKPadSimLoop) {
            kPadModule = new KPadExternalModule(name, address);
            var kPadModuleInternal = kPadModule;

            return builder
                .InNewSimulationLoop(
                    out getKPadSimLoop,
                    simLoop => simLoop.SetName(name),
                    postBuild => postBuild.ConnectExternalModule(kPadModuleInternal));
        }
    }
}

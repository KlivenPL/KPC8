using Components.Buses;
using Components.SignalMappers;
using Components.Signals;
using ExternalDevices;
using ExternalDevices.HID;
using KPC8.ControlSignals;
using Simulation.Loops;
using System;

namespace KPC8.ExternalModules {
    public class KPadExternalModule : ExternalModuleBase {

        private readonly ushort address;
        private AddressMapper addressMapper;
        private KPad kPad;
        private IExternalDevice KPadExternal => kPad;

        public KPadExternalModule(string name, ushort address) : base(name) {
            this.address = address;
        }

        public override void InitializeAndConnect(IBus dataBus, IBus addressBus, IBus interrputsBus, Signal extIn, Signal extOut) {
            addressMapper = AddressMapper.Create(16, mab => mab.Add(address));
            kPad = new KPad(name);
            KPadExternal.ConnectAsExternalDevice(extIn, extOut, addressMapper.IsMatch);
            ConnectAddressBus(addressBus);
            ConnectDataBus(dataBus);
        }

        protected override void ConnectAddressBus(IBus addressBus) {
            addressBus
                .Connect(0, 16, addressMapper.AddressInput);
        }

        protected override void ConnectDataBus(IBus dataBus) {
            dataBus
                .Connect(0, 8, kPad.Outputs);
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

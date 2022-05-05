using Components.Buses;
using Components.SignalMappers;
using Components.Signals;
using Components.Transcievers;
using ExternalDevices;
using ExternalDevices.Timers;
using KPC8.ControlSignals;
using Simulation.Loops;
using System;

namespace KPC8.ExternalModules {
    public class KTimerExternalModule : ExternalModuleBase {
        private readonly ushort address;

        #region Run at main thread
        private readonly AddressMapper extOutAddressMapper;
        private readonly HLTransciever dataBusToKTimerInputTransciever;
        // private readonly Signal enableReadSignal;
        #endregion

        #region Run at external thread
        private KTimer ktimer;
        #endregion

        private IExternalDevice KTimerExternal => ktimer;
        private Signal ktimer_cs_const;

        public KTimerStatus GetStatus() => ktimer.GetStatus();
        public float GetTimePeriod() => ktimer.GetTimePeriod();
        public byte GetIrrCode() => ktimer.GetIrrCode();

        public KTimerExternalModule(string name, ushort address) : base(name) {
            this.address = address;
            extOutAddressMapper = AddressMapper.Create(16, mab => mab.Add(address));
            dataBusToKTimerInputTransciever = new HLTransciever("KTimerInput", 8);
        }

        public override void InitializeAndConnect(IBus dataBus, IBus addressBus, IBus interrputsBus, Signal extIn, Signal extOut) {
            extOutAddressMapper.OutputEnable.PlugIn(extOut);
            ktimer = new KTimer(name);
            dataBusToKTimerInputTransciever.OutputEnable.PlugIn(extOut);

            KTimerExternal.ConnectAsExternalDevice(extIn, extOut, extOutAddressMapper.IsMatch);

            ConnectAddressBus(addressBus);
            ConnectDataBus(dataBus);
            ConnectInterruptsBus(interrputsBus);
            ConnectInternals();
        }

        protected override void ConnectAddressBus(IBus addressBus) {
            addressBus
                .Connect(0, 16, extOutAddressMapper.AddressInput);
        }

        protected override void ConnectDataBus(IBus dataBus) {
            dataBus
            .Connect(0, 8, dataBusToKTimerInputTransciever.Inputs);
        }

        protected override void ConnectInterruptsBus(IBus interruptsBus) {
            interruptsBus
               .Connect(0, 8, ktimer.Outputs);
        }

        protected override void ConnectInternals() {
            Signal.Factory.CreateAndConnectPorts("DataBusTranscieverToKTimerInput", ktimer.Inputs, dataBusToKTimerInputTransciever.Outputs);
        }
    }

    public static class KTimerExternalModuleExtensions {
        public static CpuBuilder AddKTimer(this CpuBuilder builder, string name, ushort writeAddress, out KTimerExternalModule kTimerModule, out Func<SimulationLoop> getKTimerSimLoop) {
            kTimerModule = new KTimerExternalModule(name, writeAddress);
            var KTimerModuleInternal = kTimerModule;

            return builder
                .InNewSimulationLoop(
                    out getKTimerSimLoop,
                    simLoop => simLoop.SetName(name),
                    postBuild => postBuild.ConnectExternalModule(KTimerModuleInternal));
        }
    }
}

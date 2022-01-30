using Components.Buses;
using Components.Signals;
using System;

namespace KPC8.ExternalModules {
    public abstract class ExternalModuleBase {
        protected string name;
        public ExternalModuleBase(string name) {
            this.name = name;
        }

        public abstract void InitializeAndConnect(IBus dataBus, IBus addressBus, IBus interrputsBus, Signal extIn, Signal extOut);
        protected virtual void ConnectClock(Signal clock) => throw new NotImplementedException();
        protected virtual void ConnectClockBar(Signal clockBar) => throw new NotImplementedException();
        protected virtual void ConnectInternals() => throw new NotImplementedException();
        protected virtual void CreateAndSetConstSignals() => throw new NotImplementedException();
        protected virtual void ConnectDataBus(IBus dataBus) => throw new NotImplementedException();
        protected virtual void ConnectAddressBus(IBus addressBus) => throw new NotImplementedException();
        protected virtual void ConnectInterruptsBus(IBus interruptsBus) => throw new NotImplementedException();
    }
}

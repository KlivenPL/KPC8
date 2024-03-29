﻿using Components.Buses;
using Components.Signals;
using KPC8.ControlSignals;
using System;

namespace KPC8.Modules {
    public abstract class ModuleBase<TPanel> where TPanel : ICsPanel {
        public abstract TPanel CreateControlPanel(IBus controlBus);
        protected virtual void ConnectMainClock(Signal mainClock) => throw new NotImplementedException();
        protected virtual void ConnectMainClockBar(Signal mainClockBar) => throw new NotImplementedException();
        protected virtual void ConnectInternals() => throw new NotImplementedException();
        protected virtual void CreateAndSetConstSignals() => throw new NotImplementedException();
        protected virtual void ConnectDataBus(IBus dataBus) => throw new NotImplementedException();
        protected virtual void ConnectAddressBus(IBus addressBus) => throw new NotImplementedException();
        protected virtual void ConnectRegisterSelectBus(IBus registerSelectBus) => throw new NotImplementedException();
        protected virtual void ConnectFlagsBus(IBus flagsBus) => throw new NotImplementedException();
        protected virtual void ConnectInterruptsBus(IBus interruptsBus) => throw new NotImplementedException();
    }
}

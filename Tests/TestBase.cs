using Autofac;
using Components.Clocks;
using Components.Signals;
using KPC8._Infrastructure;
using KPC8.Clocks;
using Simulation.Loops;
using System;
using System.Collections.Generic;

namespace Tests {
    public abstract class TestBase : IDisposable {
        private readonly List<Signal> cycleSignals = new List<Signal>();
        protected readonly ILifetimeScope _containerTestScope;
        protected readonly Clock _testClock;
        protected readonly SimulationLoop _testSimulationLoop;

        public TestBase() {
            _containerTestScope = CompositionRoot.BeginLifetimeScope();

            _testClock = _containerTestScope.ResolveKeyed<Clock>(ClockType.TestManualClock);
            _testSimulationLoop = _containerTestScope.Resolve<SimulationLoop>();
        }

        public void Dispose() {
            _containerTestScope.Dispose();
        }

        protected void Enable(Signal signal) {
            signal.Value = true;
            cycleSignals.Add(signal);
        }

        public void MakeTickAndWait() {
            _testSimulationLoop.Loop();
            _testClock.MakeTick();
            while (_testClock.IsManualTickInProgress) {
                _testSimulationLoop.Loop();
            }

            foreach (var sig in cycleSignals) {
                sig.Value = false;
            }

            cycleSignals.Clear();
        }
    }
}

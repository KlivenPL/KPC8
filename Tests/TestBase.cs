using Autofac;
using Components.Clocks;
using KPC8._Infrastructure;
using KPC8.Clocks;
using Simulation.Loops;
using System;

namespace Tests {
    public abstract class TestBase : IDisposable {
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

        public void MakeTickAndWait() {
            _testClock.MakeTick();
            while (_testClock.IsManualTickInProgress) {
                _testSimulationLoop.Loop();
            }
        }
    }
}

using _Infrastructure.BitArrays;
using Components.Clocks;
using Components.Signals;
using KPC8.Clocks;
using KPC8.ControlSignals;
using Simulation.Loops;
using System.Collections.Generic;

namespace Tests {
    public abstract class TestBase {
        private readonly List<Signal> cycleSignals = new List<Signal>();
        protected readonly Clock _testClock;
        private SimulationLoop _testSimulationLoop;
        private readonly SimulationLoopBuilder _simulationLoopBuilder = SimulationLoopBuilder.CreateAsCurrent().SetName("Test");
        private SimulationLoop TestSimulationLoop => _testSimulationLoop ??= _simulationLoopBuilder.Build();

        private static ClockParametersAttribute testClockParameters;
        private static ClockParametersAttribute TestClockParameters => testClockParameters ??= ClockType.TestManualClock.GetClockParameters();

        public TestBase() {
            var clk = Signal.Factory.Create("TestClock");
            var clkBar = Signal.Factory.Create("TestClockBar");

            var clkParameters = TestClockParameters;

            _testClock = new Clock(clk, clkBar, clkParameters.ClockMode, clkParameters.PeriodInTicks);
        }

        protected void Enable(Signal signal) {
            signal.Value = true;
            cycleSignals.Add(signal);
        }

        public void MakeTickAndWait() {
            TestSimulationLoop.Loop();
            _testClock.MakeTick();
            while (_testClock.IsManualTickInProgress) {
                _testSimulationLoop.Loop();
            }

            TestSimulationLoop.Loop();

            foreach (var sig in cycleSignals) {
                sig.Value = false;
            }

            cycleSignals.Clear();
        }


        public void MakeOnlyLoops() {
            TestSimulationLoop.Loop();
            TestSimulationLoop.Loop();
            TestSimulationLoop.Loop();
            TestSimulationLoop.Loop();
        }

        protected string GetCsErrorMessage(int step, ControlSignalType expectedSignal, IEnumerable<Signal> actual)
            => $"Failed at step: {step}\r\nExpected control signal:\t{expectedSignal}\r\nActual control signal:\t\t{ControlSignalTypeExtensions.FromBitArray(actual.ToBitArray())}\r\n";
    }
}

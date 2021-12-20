using Components.Signals;
using Simulation.Frames;
using Simulation.Updates;
using System;

namespace Components.Clocks {
    public class Clock : IUpdate {
        private long timer = 0L;
        private ClockMode mode;

        public Clock(Signal signal, ClockMode mode, long periodInTicks) {
            Mode = mode;
            PeriodInTicks = periodInTicks;
            Clk = signal;

            this.RegisterUpdate();
            // Console.WriteLine($"Running clock in {1 / ((decimal)periodInTicks / Stopwatch.Frequency) / 1000} KHz");
        }

        public long PeriodInTicks { get; set; }
        public ClockMode Mode {
            get => mode;
            set {
                if (value == ClockMode.Automatic) {
                    timer = 0L;
                } else if (value == ClockMode.Manual) {
                    timer = -1L;
                }

                mode = value;
            }
        }

        public Signal Clk {
            get;
            private set;
        }

        public bool IsManualTickInProgress => timer >= 0L && Mode == ClockMode.Manual;

        public void MakeTick() {
            if (Mode == ClockMode.Automatic) {
                throw new Exception("Invalid mode - automatic.");
            }

            if (IsManualTickInProgress) {
                Console.WriteLine("Warn: (clock) Tick in progress");
                return;
            }

            timer = 0L;
        }

        public void Update() {
            switch (Mode) {
                case ClockMode.Automatic:
                    AutomaticUpdate();
                    break;
                case ClockMode.Manual:
                    ManualUpdate();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void AutomaticUpdate() {
            timer += FrameInfo.DeltaTicks;

            Clk.Value = timer <= PeriodInTicks / 2L;

            if (timer >= PeriodInTicks) {
                timer = 0L;
            }
        }

        private void ManualUpdate() {
            if (IsManualTickInProgress) {
                timer += FrameInfo.DeltaTicks;
            }

            Clk.Value = timer <= PeriodInTicks / 2L && IsManualTickInProgress;

            if (timer >= PeriodInTicks) {
                timer = -1L;
            }
        }
    }
}

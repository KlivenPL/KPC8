using Components.Signals;
using Simulation.Updates;
using System;
using System.Diagnostics;

namespace Components.Clocks {
    public class Clock : IUpdate {
        private long timer = 0L;
        private ClockMode mode;
        private Stopwatch sw;

        private long cycles = 0;

        public Clock(Signal signal, ClockMode mode, long periodInTicks) {
            sw = new Stopwatch();
            Mode = mode;
            PeriodInTicks = periodInTicks;
            Clk = signal;
            this.RegisterUpdate();
        }

        public long PeriodInTicks { get; set; }
        public ClockMode Mode {
            get => mode;
            set {
                if (value == ClockMode.Automatic) {
                    timer = 0L;
                    sw.Restart();
                } else if (value == ClockMode.Manual) {
                    timer = -1L;
                    sw.Stop();
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
            timer += 1;

            Clk.Value = timer <= PeriodInTicks / 2L;

            if (timer >= PeriodInTicks) {
                if (++cycles % 10000000 == 0) {
                    Console.WriteLine($"Running clock in { 1L / ((decimal)sw.ElapsedTicks / cycles / Stopwatch.Frequency) / 1000000:f} MHz");
                    cycles = 0;
                    sw.Restart();
                }

                timer = 0L;
            }
        }

        private void ManualUpdate() {
            if (IsManualTickInProgress) {
                timer += 1;
            }

            Clk.Value = timer <= PeriodInTicks / 2L && IsManualTickInProgress;

            if (timer >= PeriodInTicks) {
                timer = -1L;
            }
        }
    }
}

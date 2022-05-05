using Components.Signals;
using Simulation.Updates;
using System;

namespace Components.Clocks {
    public class Clock : IUpdate {
        public int Priority => 10;
        private long timer = 0L;
        private ClockMode mode;
        //private Stopwatch sw;

        public long Cycles { get; private set; } = 0;
        private long halfPeriod = 0;

        public bool Disabled { get; set; } = false;
        /*public event Action<Clock> OnCycle;*/

        public Clock(Signal clkSignal, Signal clkBarSignal, ClockMode mode, long periodInTicks) {
            //    sw = new Stopwatch();
            Mode = mode;
            PeriodInTicks = periodInTicks;
            halfPeriod = PeriodInTicks / 2L;
            Clk = clkSignal;
            ClkBar = clkBarSignal;

            Clk.SetValueWithoutTriggeringEvents(false);
            ClkBar.SetValueWithoutTriggeringEvents(true);
            this.RegisterUpdate();
        }

        public long PeriodInTicks { get; set; }
        public ClockMode Mode {
            get => mode;
            set {
                if (value == ClockMode.Automatic) {
                    timer = 0L;
                    //  sw.Restart();
                } else if (value == ClockMode.Manual) {
                    timer = -1L;
                    //  sw.Stop();
                }

                mode = value;
            }
        }

        public Signal Clk { get; private set; }
        public Signal ClkBar { get; private set; }

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
            if (Disabled) {
                return;
            }

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
            Clk.Value = timer == 1;
            ClkBar.Value = !Clk.Value;

            timer += 1;
            if (timer >= PeriodInTicks) {
                /*if (++Cycles % 100000 == 0) {
                    var t = (decimal)sw.ElapsedTicks / Cycles / Stopwatch.Frequency;
                    Console.WriteLine($"Running clock in { 1L / t / 1000:f} KHz ({t * 1000000000:f}ns)");
                    Cycles = 0;
                    sw.Restart();
                }

                OnCycle?.Invoke(this);*/
                timer = 0L;
            }
        }

        private void ManualUpdate() {
            if (IsManualTickInProgress) {
                timer += 1;
            }

            Clk.Value = timer <= halfPeriod && IsManualTickInProgress;
            ClkBar.Value = !Clk.Value;

            if (timer >= PeriodInTicks) {
                /*OnCycle?.Invoke(this);*/
                timer = -1L;
            }
        }

        public void Dispose() {
            this.UnregisterUpdate();
        }

        public override string ToString() {
            return $"Clk: {(Clk ? "1" : "0")}, Bar: {(ClkBar ? "1" : "0")}, Mode: {Mode}";
        }
    }
}

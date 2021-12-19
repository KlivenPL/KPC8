using Components.Signals;
using Simulation.Frames;
using Simulation.Updates;
using System;

namespace Components.Clocks {
    public class Clock : IUpdate {
        private double timer = 0.0;
        private ClockMode mode;

        public Clock(Signal signal, ClockMode mode, double frequency) {
            Mode = mode;
            Frequency = frequency;
            Clk = signal;

            this.RegisterUpdate();
        }

        public double Frequency { get; set; }
        public ClockMode Mode {
            get => mode;
            set {
                if (value == ClockMode.Automatic) {
                    timer = 0.0;
                } else if (value == ClockMode.Manual) {
                    timer = -1.0;
                }

                mode = value;
            }
        }

        public Signal Clk {
            get;
            private set;
        }

        public double Period => 1.0 / Frequency;

        public bool IsManualTickInProgress => timer >= 0.0 && Mode == ClockMode.Manual;

        public void MakeTick() {
            if (Mode == ClockMode.Automatic) {
                throw new Exception("Invalid mode - automatic.");
            }

            if (IsManualTickInProgress) {
                Console.WriteLine("Warn: (clock) Tick in progress");
                return;
            }

            timer = 0.0;
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
            timer += FrameInfo.DeltaTime;

            Clk.Value = timer <= Period / 2.0;

            if (timer >= Period) {
                timer = 0.0;
            }
        }

        private void ManualUpdate() {
            if (IsManualTickInProgress) {
                timer += FrameInfo.DeltaTime;
            }

            Clk.Value = timer <= Period / 2.0 && IsManualTickInProgress;

            if (timer >= Period) {
                timer = -1.0;
            }
        }
    }
}

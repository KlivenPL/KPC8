using LightweightEmulator.Configuration;
using LightweightEmulator.ExternalDevices;
using LightweightEmulator.Pipelines;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExternalDevices.Lw {
    public class LwKTimerConfiguration : ILwKpcExternalInterruptDeviceConfiguration {
        public string TimerName { get; init; }
        public ushort TimerAddress { get; init; }

        public void Configure(Action<ILwExternalDevice> addExternalDevice, TryQueueInterruptDelegate tryQueueInterrupt) {
            var lwKTimer = new LwKTimer(tryQueueInterrupt, TimerAddress, TimerName);
            addExternalDevice(lwKTimer);
        }
    }
    public class LwKTimer : LwExternalInterruptDevice {
        private byte irrCode;
        private int _frequency; // in Hz, e.g. 1 = 1Hz, 100 = 100Hz
        private bool _enabled;
        private CancellationTokenSource _cts;
        private Task _timerTask;
        private TaskCompletionSource<bool> _ackTcs = new TaskCompletionSource<bool>();
        private readonly object _lock = new object();
        private Action abortInterrupt;

        public LwKTimer(TryQueueInterruptDelegate tryQueueInterruptDelegate, ushort address, string name)
            : base(tryQueueInterruptDelegate, name) {
            MappedAddresses.Add(address);
        }

        public override void HandleSbext(ushort address, byte data) {
            Enabled = false;
            if (data == 0) {
                Enabled = false;
            } else if (data >= 0xF0) {
                irrCode = (byte)(0x0F & data);
            } else {
                Frequency = data;
                Enabled = true;
            }
        }

        public override Task HandleInterruptReady() {
            _ackTcs.TrySetResult(true);
            return Task.CompletedTask;
        }

        public void OnTick() {
            TryQueueInterrupt(irrCode, out abortInterrupt);
        }

        // Frequency in Hz. Can be changed at runtime.
        public int Frequency {
            get { lock (_lock) { return _frequency; } }
            set {
                if (value <= 0)
                    throw new ArgumentException("Frequency must be greater than 0");

                lock (_lock) {
                    _frequency = value;
                }
            }
        }

        // Enabled property. When set, the timer is started or stopped accordingly.
        public bool Enabled {
            get { lock (_lock) { return _enabled; } }
            set {
                lock (_lock) {
                    if (_enabled == value) return;
                    _enabled = value;
                    if (_enabled)
                        StartTimer();
                    else
                        StopTimer();
                }
            }
        }

        // Starts the timer loop.
        private void StartTimer() {
            _cts = new CancellationTokenSource();
            _timerTask = Task.Run(() => TimerLoop(_cts.Token));
        }

        // The asynchronous timer loop.
        private async Task TimerLoop(CancellationToken token) {
            while (!token.IsCancellationRequested) {
                // Create a new TaskCompletionSource for this tick.
                _ackTcs = new TaskCompletionSource<bool>();

                // Compute delay based on the current frequency.
                int delayMs = (int)(1000.0 / Frequency);
                try {
                    await Task.Delay(delayMs, token);
                } catch (TaskCanceledException) {
                    // Timer was cancelled
                    break;
                }

                // Fire the tick event.
                OnTick();

                // Wait for the external acknowledgment.
                await _ackTcs.Task;
            }
        }

        // Stops the timer loop.
        private void StopTimer() {
            _cts?.Cancel();
            _ackTcs?.SetCanceled();
            abortInterrupt?.Invoke();
        }

        public override void Dispose() {
            StopTimer();
        }
    }
}

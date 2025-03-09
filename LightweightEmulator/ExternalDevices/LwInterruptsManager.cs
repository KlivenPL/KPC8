namespace LightweightEmulator.ExternalDevices {
    public class LwInterruptsManager {
        private static Dictionary<byte, ushort> _irrCodeToAddress = new() {
            { 0b0000, 0xFFF0 },
            { 0b0001, 0xFFE0 },
            { 0b0010, 0xFFD0 },
            { 0b0011, 0xFFC0 },
            { 0b0100, 0xFFB0 },
            { 0b0101, 0xFFA0 },
            { 0b0110, 0xFF90 },
            { 0b0111, 0xFF80 },
            { 0b1000, 0xFF70 },
            { 0b1001, 0xFF60 },
            { 0b1010, 0xFF50 },
            { 0b1011, 0xFF40 },
            { 0b1100, 0xFF30 },
            { 0b1101, 0xFF20 },
            { 0b1110, 0xFF10 },
            { 0b1111, 0xFF00 }
        };

        private readonly List<IrrRequest> _irrQueue = new();
        private IrrRequest? _handledInterrupt;

        private readonly object _irrQueueLock = new();
        private readonly object _handledInterruptLock = new();

        public bool En { get; private set; }
        public bool Busy {
            get {
                lock (_handledInterruptLock) {
                    return _handledInterrupt != null;
                }
            }
        }

        public bool TryQueueInterrupt(
            byte fourBitIrrCode,
            Func<Task>? interruptRdyCallback,
            out Action? abortIrrRequest) {

            if ((fourBitIrrCode & 0b11110000) != 0) {
                throw new Exception("Irr code must be a 4 bit code.");
            }

            abortIrrRequest = null;

            IrrRequest irrRequest = new() {
                FourBitIrrCode = fourBitIrrCode,
                InterruptReadyCallback = interruptRdyCallback
            };

            lock (_irrQueueLock) {
                abortIrrRequest = () => AbortIrrRequest(irrRequest);
                _irrQueue.Add(irrRequest);
            }

            return true;
        }

        internal bool ShouldProcessInterrupt(out ushort? irrAddress, out Action? handleIrrex) {
            irrAddress = null;
            handleIrrex = null;

            if (!En || Busy) {
                return false;
            }

            lock (_irrQueueLock) {
                var irrRequest = _irrQueue.FirstOrDefault();

                if (irrRequest == null) {
                    return false;
                }

                irrAddress = _irrCodeToAddress[irrRequest.FourBitIrrCode];
                handleIrrex = () => HandleIrrex(irrRequest);
            }

            return true;
        }

        internal void HandleIrrret() {
            Func<Task>? callback = null;

            lock (_handledInterruptLock) {
                if (_handledInterrupt == null) {
                    throw new Exception("At this stage, _currentlyHandledInterrupt should not be null");
                }

                if (_handledInterrupt.Aborted) {
                    return;
                }

                callback = _handledInterrupt.InterruptReadyCallback;
                _handledInterrupt = null;
            }

            ExecuteInterruptReadyCallback(callback);
        }

        internal void HandleIrren() {
            En = true;
        }

        internal void HandleIrrdis() {
            En = false;
        }

        private void AbortIrrRequest(IrrRequest irrRequestToAbort) {
            if (irrRequestToAbort == null) {
                return;
            }

            irrRequestToAbort.Abort();

            lock (_irrQueueLock) {
                _irrQueue.RemoveAll(x => x == irrRequestToAbort);
            }
        }

        private void HandleIrrex(IrrRequest irrRequest) {
            lock (_handledInterruptLock) {
                _handledInterrupt = irrRequest;
            }

            lock (_irrQueueLock) {
                _irrQueue.RemoveAll(_x => _x == irrRequest);
            }
        }

        private static async void ExecuteInterruptReadyCallback(Func<Task>? callback) {
            if (callback != null) {
                await callback();
            }
        }

        private class IrrRequest {
            public byte FourBitIrrCode { get; init; }
            public Func<Task>? InterruptReadyCallback { get; init; }
            public bool Aborted { get; private set; } = false;

            public void Abort() {
                Aborted = true;
            }
        }
    }
}

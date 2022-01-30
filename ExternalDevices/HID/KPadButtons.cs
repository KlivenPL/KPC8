using System;

namespace ExternalDevices.HID {
    [Flags]
    public enum KPadButtons : byte {
        None = 0,
        Up = 1,
        Right = 2,
        Down = 4,
        Left = 8,
        A = 16,
        B = 32,
        Start = 64,
        Select = 128
    }
}

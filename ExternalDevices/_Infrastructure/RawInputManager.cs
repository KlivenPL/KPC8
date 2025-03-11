using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Threading;

namespace ExternalDevices._Infrastructure {
    internal class RawInputEvent {
        public bool IsRecognizedVirtualKey(out VirtualKey virtualKey) {
            virtualKey = default;
            if (Enum.IsDefined(typeof(VirtualKey), VirtualKeyCode)) {
                virtualKey = (VirtualKey)VirtualKeyCode;
                return true;
            }
            return false;
        }

        public int VirtualKeyCode { get; set; }
        public ushort MakeCode { get; set; }
        public ushort Flags { get; set; }
        public bool IsKeyDown { get; set; }
        public DateTime Timestamp { get; set; }
    }

    // This class is independent of Forms and creates its own hidden window
    // to receive raw input. It also disambiguates common keys like Shift and Control.
    internal class RawInputManager : IDisposable {
        // Singleton instance.
        private static readonly Lazy<RawInputManager> _instance = new Lazy<RawInputManager>(() => new RawInputManager());
        public static RawInputManager Instance => _instance.Value;

        // Thread and synchronization fields.
        private readonly Thread messageLoopThread;
        private readonly AutoResetEvent windowCreated = new AutoResetEvent(false);
        private volatile bool disposed = false;

        // The handle for the created hidden window.
        private IntPtr windowHandle = IntPtr.Zero;

        // Thread-safe queue for raw input events.
        private readonly ConcurrentQueue<RawInputEvent> eventQueue = new ConcurrentQueue<RawInputEvent>();

        // The OS thread ID of the message loop thread.
        private uint messageLoopThreadId = 0;

        // Delegate for our window procedure. (Keep a reference to avoid GC.)
        private readonly WndProcDelegate wndProcDelegate;
        private GCHandle wndProcHandle;

        // Constants for raw input and window messages.
        private const int WM_INPUT = 0x00FF;
        private const uint RID_INPUT = 0x10000003;
        private const ushort RI_KEY_BREAK = 0x01;
        private const ushort RI_KEY_E0 = 0x02;
        private const uint WM_QUIT = 0x0012;
        private const string WINDOW_CLASS_NAME = "RawInputManagerWindowClass";

        // Constructor is private for singleton.
        private RawInputManager() {
            wndProcDelegate = new WndProcDelegate(WndProc);
            // Pin the delegate to prevent GC.
            wndProcHandle = GCHandle.Alloc(wndProcDelegate, GCHandleType.Normal);

            messageLoopThread = new Thread(MessageLoop) {
                IsBackground = true
            };
            messageLoopThread.Start();
            // Wait until the window is created and raw input is registered.
            windowCreated.WaitOne();
        }

        /// <summary>
        /// Attempts to dequeue a raw input event.
        /// Returns true if an event was available; otherwise, false.
        /// </summary>
        public bool TryDequeue(out RawInputEvent rawEvent) {
            return eventQueue.TryDequeue(out rawEvent);
        }

        // The window procedure delegate type.
        private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam);

        // Structure definitions for registering a window class.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WNDCLASSEX {
            public uint cbSize;
            public uint style;
            public IntPtr lpfnWndProc; // pointer to window procedure
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string lpszMenuName;
            public string lpszClassName;
            public IntPtr hIconSm;
        }

        // Structure for Windows messages.
        [StructLayout(LayoutKind.Sequential)]
        private struct MSG {
            public IntPtr hwnd;
            public uint message;
            public UIntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public POINT pt;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT {
            public int x;
            public int y;
        }

        // P/Invoke declarations for window class registration and message loop.
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool UnregisterClass(string lpClassName, IntPtr hInstance);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr CreateWindowEx(
            uint dwExStyle,
            string lpClassName,
            string lpWindowName,
            uint dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern sbyte GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll")]
        private static extern bool TranslateMessage([In] ref MSG lpMsg);

        [DllImport("user32.dll")]
        private static extern IntPtr DispatchMessage([In] ref MSG lpmsg);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        // The message loop thread method: registers a window class, creates a hidden window,
        // registers for raw input, and runs the loop.
        private void MessageLoop() {
            // Capture this thread’s OS thread id.
            messageLoopThreadId = GetCurrentThreadId();

            IntPtr hInstance = GetModuleHandle(null);

            // Register window class. Use the ERROR_CLASS_ALREADY_EXISTS approach.
            WNDCLASSEX wndClass = new WNDCLASSEX {
                cbSize = (uint)Marshal.SizeOf(typeof(WNDCLASSEX)),
                style = 0,
                lpfnWndProc = Marshal.GetFunctionPointerForDelegate(wndProcDelegate),
                cbClsExtra = 0,
                cbWndExtra = 0,
                hInstance = hInstance,
                hIcon = IntPtr.Zero,
                hCursor = IntPtr.Zero,
                hbrBackground = IntPtr.Zero,
                lpszMenuName = null,
                lpszClassName = WINDOW_CLASS_NAME,
                hIconSm = IntPtr.Zero
            };

            ushort regResult = RegisterClassEx(ref wndClass);
            if (regResult == 0) {
                int err = Marshal.GetLastWin32Error();
                // If the class already exists, ignore the error.
                if (err != 1410) {
                    throw new Exception("Failed to register window class. Error code: " + err);
                }
            }

            // Create a hidden window.
            windowHandle = CreateWindowEx(
                0,
                WINDOW_CLASS_NAME,
                "RawInputManagerWindow",
                0,
                0, 0, 0, 0,
                IntPtr.Zero, // no parent
                IntPtr.Zero,
                hInstance,
                IntPtr.Zero);

            if (windowHandle == IntPtr.Zero) {
                throw new Exception("Failed to create message window.");
            }

            // Register for raw keyboard input.
            RegisterForRawInput(windowHandle);

            // Signal that the window is ready.
            windowCreated.Set();

            // Run the message loop.
            MSG msg;
            while (GetMessage(out msg, IntPtr.Zero, 0, 0) != 0) {
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }
            // Do not unregister the class here.
        }

        // Structures and P/Invoke for raw input.
        [StructLayout(LayoutKind.Sequential)]
        private struct RAWINPUTHEADER {
            public uint Type;
            public uint Size;
            public IntPtr Device;
            public IntPtr wParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RAWKEYBOARD {
            public ushort MakeCode;
            public ushort Flags;
            public ushort Reserved;
            public ushort VirtualKey;
            public uint Message;
            public uint ExtraInformation;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RAWINPUT {
            public RAWINPUTHEADER header;
            public RAWKEYBOARD keyboard;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterRawInputDevices(
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] RAWINPUTDEVICE[] pRawInputDevices,
            uint uiNumDevices,
            uint cbSize);

        [StructLayout(LayoutKind.Sequential)]
        private struct RAWINPUTDEVICE {
            public ushort UsagePage;
            public ushort Usage;
            public uint Flags;
            public IntPtr hwndTarget;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetRawInputData(
            IntPtr hRawInput,
            uint uiCommand,
            IntPtr pData,
            ref uint pcbSize,
            uint cbSizeHeader);

        // Registers the window to receive raw keyboard input.
        private void RegisterForRawInput(IntPtr hwnd) {
            RAWINPUTDEVICE[] rid = new RAWINPUTDEVICE[1];
            rid[0].UsagePage = 0x01; // Generic desktop controls.
            rid[0].Usage = 0x06;     // Keyboard.
            rid[0].Flags = 0;        // Optionally, add RIDEV_INPUTSINK if needed.
            rid[0].hwndTarget = hwnd;

            if (!RegisterRawInputDevices(rid, (uint)rid.Length, (uint)Marshal.SizeOf(typeof(RAWINPUTDEVICE)))) {
                throw new Exception("Failed to register raw input device.");
            }
        }

        // The window procedure: processes WM_INPUT messages.
        private IntPtr WndProc(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam) {
            if (msg == WM_INPUT) {
                uint dwSize = 0;
                // First call with null to determine the size.
                GetRawInputData(lParam, RID_INPUT, IntPtr.Zero, ref dwSize, (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER)));
                IntPtr buffer = Marshal.AllocHGlobal((int)dwSize);
                try {
                    if (GetRawInputData(lParam, RID_INPUT, buffer, ref dwSize, (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER))) == dwSize) {
                        RAWINPUT raw = Marshal.PtrToStructure<RAWINPUT>(buffer);
                        // Process only keyboard input (Type == 1)
                        if (raw.header.Type == 1) {
                            // Resolve the virtual key to account for keys like Shift, Control, Alt, etc.
                            int resolvedVKey = ResolveVirtualKey(raw.keyboard);

                            var rawEvent = new RawInputEvent {
                                VirtualKeyCode = resolvedVKey,
                                MakeCode = raw.keyboard.MakeCode,
                                Flags = raw.keyboard.Flags,
                                IsKeyDown = (raw.keyboard.Flags & RI_KEY_BREAK) == 0,
                                Timestamp = DateTime.Now
                            };
                            eventQueue.Enqueue(rawEvent);
                        }
                    }
                } finally {
                    Marshal.FreeHGlobal(buffer);
                }
            }
            return DefWindowProc(hWnd, msg, wParam, lParam);
        }

        // Helper method to disambiguate modifier and special keys.
        private int ResolveVirtualKey(RAWKEYBOARD rawKeyboard) {
            int vkey = rawKeyboard.VirtualKey;

            if (vkey == 0x10) {
                if (rawKeyboard.MakeCode == 0x2A)
                    return 0xA0; // VK_LSHIFT
                else if (rawKeyboard.MakeCode == 0x36)
                    return 0xA1; // VK_RSHIFT
            } else if (vkey == 0x11) {
                if ((rawKeyboard.Flags & RI_KEY_E0) != 0)
                    return 0xA3; // VK_RCONTROL
                else
                    return 0xA2; // VK_LCONTROL
            } else if (vkey == 0x12) {
                if ((rawKeyboard.Flags & RI_KEY_E0) != 0)
                    return 0xA5; // VK_RMENU
                else
                    return 0x12; // VK_MENU
            }
            return vkey;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostThreadMessage(uint idThread, uint Msg, UIntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Disposes the manager by posting a WM_QUIT message to its message loop.
        /// Note: In a singleton scenario, you might not call Dispose until application termination.
        /// </summary>
        public void Dispose() {
            if (!disposed) {
                PostThreadMessage(messageLoopThreadId, WM_QUIT, UIntPtr.Zero, IntPtr.Zero);
                messageLoopThread.Join();
                wndProcHandle.Free();
                disposed = true;
            }
        }
    }
}

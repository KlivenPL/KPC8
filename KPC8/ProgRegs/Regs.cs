using System;

namespace KPC8.ProgRegs {
    [Flags]
    public enum Regs : ushort {
        None = 0,

        /// <summary>
        /// Always 0
        /// </summary>
        Zero = 1,

        /// <summary>
        /// Temporary 4
        /// </summary>
        T4 = 1 << 1,

        /// <summary>
        /// Stack pointer
        /// </summary>
        Sp = 1 << 2,

        /// <summary>
        /// Frame pointer
        /// </summary>
        Fp = 1 << 3,

        /// <summary>
        /// Temporary 1
        /// </summary>
        T1 = 1 << 4,

        /// <summary>
        /// Temporary 2
        /// </summary>
        T2 = 1 << 5,

        /// <summary>
        /// Temporary 3
        /// </summary>
        T3 = 1 << 6,

        /// <summary>
        /// Reserved for assembler
        /// </summary>
        Ass = 1 << 7,

        /// <summary>
        /// Saved 1
        /// </summary>
        S1 = 1 << 8,

        /// <summary>
        /// Saved 2
        /// </summary>
        S2 = 1 << 9,

        /// <summary>
        /// Saved 3
        /// </summary>
        S3 = 1 << 10,

        /// <summary>
        /// Function arg 1
        /// </summary>
        A1 = 1 << 11,

        /// <summary>
        /// Function arg 2
        /// </summary>
        A2 = 1 << 12,

        /// <summary>
        /// Function arg 3
        /// </summary>
        A3 = 1 << 13,

        /// <summary>
        /// Result of function
        /// </summary>
        Rt = 1 << 14,

        /// <summary>
        /// Return address
        /// </summary>
        Ra = 1 << 15
    }
}

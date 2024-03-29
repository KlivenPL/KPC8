﻿using System;

namespace KPC8.CpuFlags {
    [Flags]
    public enum CpuFlag : byte {
        None = 0,

        /// <summary>
        /// Zero flag
        /// </summary>
        Zf = 1,

        /// <summary>
        /// Negative flag
        /// </summary>
        Nf = 2,

        /// <summary>
        /// Carry flag
        /// </summary>
        Cf = 4,

        /// <summary>
        /// Overflow flag
        /// </summary>
        Of = 8
    }
}

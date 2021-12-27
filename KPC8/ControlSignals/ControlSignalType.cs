using System;

namespace KPC8.ControlSignals {
    [Flags]
    public enum ControlSignalType : uint {
        None = 0,
        Pc_le_hi = 1 << 0,
        Pc_le_lo = 1 << 1,
        // Pc_le = 1 << 2,
        Pc_oe = 1 << 3,
        Pc_ce = 1 << 4,
        Mar_le_hi = 1 << 5,
        Mar_le_lo = 1 << 6,
        // Mar_le = 1 << 7,
        MarToBus_oe = 1 << 8,
        Mar_ce = 1 << 9,
        Ram_we = 1 << 10,
        Ram_oe = 1 << 11,
        Rom_oe = 1 << 12,
        Ir_le_hi = 1 << 13,
        Ir_le_lo = 1 << 14,
        Ir8LSBToBus_oe = 1 << 15,
        Ic_clr = 1 << 16,
        DecDest_oe = 1 << 17,
        DecA_oe = 1 << 18,
        DecB_oe = 1 << 19,
        Regs_le = 1 << 20,
        Regs_oe = 1 << 21,
        RegA_le = 1 << 22,
        RegAToBus_oe = 1 << 23,
        RegB_le = 1 << 24,
        RegBToBus_oe = 1 << 25,
        Alu_oe = 1 << 26,
        Alu_sube = 1 << 27,
    }
}

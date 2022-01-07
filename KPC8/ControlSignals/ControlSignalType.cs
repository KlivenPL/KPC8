using System;

namespace KPC8.ControlSignals {
    [Flags]
    public enum ControlSignalType : uint {
        None = 0b00000000000000000000000000000000,
        Pc_le_hi = 0b00000000000000000000000000000001,
        Pc_le_lo = 0b00000000000000000000000000000010,
        unassigned5 = 0b00000000000000000000000000000100,
        Pc_oe = 0b00000000000000000000000000001000,
        Pc_ce = 0b00000000000000000000000000010000,
        Mar_le_hi = 0b00000000000000000000000000100000,
        Mar_le_lo = 0b00000000000000000000000001000000,
        unassigned6 = 0b00000000000000000000000010000000,
        MarToBus_oe = 0b00000000000000000000000100000000,
        Mar_ce = 0b00000000000000000000001000000000,
        Ram_we = 0b00000000000000000000010000000000,
        Ram_oe = 0b00000000000000000000100000000000,
        Rom_oe = 0b00000000000000000001000000000000,
        Ir_le_hi = 0b00000000000000000010000000000000,
        Ir_le_lo = 0b00000000000000000100000000000000,
        Ir8LSBToBus_oe = 0b00000000000000001000000000000000,
        Ic_clr = 0b00000000000000010000000000000000,
        DecDest_oe = 0b00000000000000100000000000000000,
        DecA_oe = 0b00000000000001000000000000000000,
        DecB_oe = 0b00000000000010000000000000000000,
        Regs_L = 0b00000000000100000000000000000000,
        Regs_O = 0b00000000001000000000000000000000,
        Regs_H = 0b00000000010000000000000000000000,
        RegA_le = 0b00000000100000000000000000000000,
        RegAToBus_oe = 0b00000001000000000000000000000000,
        RegB_le = 0b00000010000000000000000000000000,
        RegBToBus_oe = 0b00000100000000000000000000000000,
        Alu_oe = 0b00001000000000000000000000000000,
        Alu_sube = 0b00010000000000000000000000000000,
        unassigned2 = 0b00100000000000000000000000000000,
        unassigned3 = 0b01000000000000000000000000000000,
        unassigned4 = 0b10000000000000000000000000000000,
    }

    /// <summary>
    /// DO NOT COMBINE FURTHER!
    /// </summary>
    public static class CombinedControlSignals {
        public const ControlSignalType Regs_le_lo = ControlSignalType.Regs_L;
        public const ControlSignalType Regs_le_hi = ControlSignalType.Regs_L | ControlSignalType.Regs_H;
        public const ControlSignalType Regs_le_lo_hi = ControlSignalType.Regs_L | ControlSignalType.Regs_O;

        public const ControlSignalType Regs_oe_lo = ControlSignalType.Regs_O;
        public const ControlSignalType Regs_oe_hi = ControlSignalType.Regs_O | ControlSignalType.Regs_H;
        public const ControlSignalType Regs_oe_lo_hi = ControlSignalType.Regs_H;

        public const ControlSignalType Regs_clr = ControlSignalType.Regs_L | ControlSignalType.Regs_O | ControlSignalType.Regs_H;
    }
}

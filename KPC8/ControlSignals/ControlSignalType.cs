using System;

namespace KPC8.ControlSignals {
    [Flags]
    public enum ControlSignalType : ulong {
        None                =       0b0000000000000000000000000000000000000000,
        Pc_le_hi            =       0b0000000000000000000000000000000000000001,
        Pc_le_lo            =       0b0000000000000000000000000000000000000010,
        Pc_oe               =       0b0000000000000000000000000000000000000100,
        Pc_ce               =       0b0000000000000000000000000000000000001000,
        Mar_le_hi           =       0b0000000000000000000000000000000000010000,
        Mar_le_lo           =       0b0000000000000000000000000000000000100000,
        MarToBus_oe         =       0b0000000000000000000000000000000001000000,
        Mar_ce              =       0b0000000000000000000000000000000010000000,
        Ram_we              =       0b0000000000000000000000000000000100000000,
        Ram_oe              =       0b0000000000000000000000000000001000000000,
        Rom_oe              =       0b0000000000000000000000000000010000000000,
        Ir_le_hi            =       0b0000000000000000000000000000100000000000,
        Ir_le_lo            =       0b0000000000000000000000000001000000000000,
        Ir8LSBToBus_oe      =       0b0000000000000000000000000010000000000000,
        Ic_clr              =       0b0000000000000000000000000100000000000000,
        DecDest_oe          =       0b0000000000000000000000001000000000000000,
        DecA_oe             =       0b0000000000000000000000010000000000000000,
        DecB_oe             =       0b0000000000000000000000100000000000000000,
        Irr_a               =       0b0000000000000000000001000000000000000000,
        Irr_b               =       0b0000000000000000000010000000000000000000,
        Regs_L              =       0b0000000000000000000100000000000000000000,
        Regs_O              =       0b0000000000000000001000000000000000000000,
        Regs_H              =       0b0000000000000000010000000000000000000000,
        RegA_le             =       0b0000000000000000100000000000000000000000,
        RegAToBus_oe        =       0b0000000000000001000000000000000000000000,
        RegB_le             =       0b0000000000000010000000000000000000000000,
        RegBToBus_oe        =       0b0000000000000100000000000000000000000000,
        Alu_oe              =       0b0000000000001000000000000000000000000000,
        Alu_a               =       0b0000000000010000000000000000000000000000,
        Alu_b               =       0b0000000000100000000000000000000000000000,
        Alu_c               =       0b0000000001000000000000000000000000000000,
        AddrToData_hi       =       0b0000000010000000000000000000000000000000,
        AddrToData_lo       =       0b0000000100000000000000000000000000000000,
        Ext_in              =       0b0000001000000000000000000000000000000000,
        Ext_out             =       0b0000010000000000000000000000000000000000,
        FlagsToDataBus_oe   =       0b0000100000000000000000000000000000000000,
		DataBusToFlags_le   =       0b0001000000000000000000000000000000000000,
        /// <summary>
        /// Great power comes with great responsibility. Make sure that usage won't fuck things up.
        /// </summary>
        MODIFIER =       0b1000000000000000000000000000000000000000, 
    }

    public static class CombinedControlSignals {
        #region Regs

        public const ControlSignalType Regs_le_lo = ControlSignalType.Regs_L;
        public const ControlSignalType Regs_le_hi = ControlSignalType.Regs_L | ControlSignalType.Regs_H;
        public const ControlSignalType Regs_le_lo_hi = ControlSignalType.Regs_L | ControlSignalType.Regs_O;

        public const ControlSignalType Regs_oe_lo = ControlSignalType.Regs_O;
        public const ControlSignalType Regs_oe_hi = ControlSignalType.Regs_O | ControlSignalType.Regs_H;
        public const ControlSignalType Regs_oe_lo_hi = ControlSignalType.Regs_H;

        public const ControlSignalType Regs_clr = ControlSignalType.Regs_L | ControlSignalType.Regs_O | ControlSignalType.Regs_H;

        #endregion

        #region Irr

        public const ControlSignalType Irr_en = ControlSignalType.Irr_b;
        public const ControlSignalType Irr_dis = ControlSignalType.Irr_a;
        public const ControlSignalType Irr_ack_toggle = ControlSignalType.Irr_a | ControlSignalType.Irr_b;

        #endregion

        #region Alu

        public const ControlSignalType Alu_sub = ControlSignalType.Alu_c;
        public const ControlSignalType Alu_not = ControlSignalType.Alu_b;
        public const ControlSignalType Alu_or = ControlSignalType.Alu_b | ControlSignalType.Alu_c;
        public const ControlSignalType Alu_and = ControlSignalType.Alu_a;
        public const ControlSignalType Alu_xor = ControlSignalType.Alu_a | ControlSignalType.Alu_c;
        public const ControlSignalType Alu_sl = ControlSignalType.Alu_a | ControlSignalType.Alu_b;
        public const ControlSignalType Alu_sr = ControlSignalType.Alu_a | ControlSignalType.Alu_b | ControlSignalType.Alu_c;

        #endregion

        #region Modifier

        public const ControlSignalType MODIFIER_Alu_carry_en = ControlSignalType.MODIFIER;

        #endregion
    }
}

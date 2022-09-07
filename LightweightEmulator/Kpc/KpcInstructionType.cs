namespace LightweightEmulator.Kpc {
    public enum KpcInstructionType : ushort {
        Nop = 0x0,

        #region Load
        Lbrom = 0x01,
        Lbromo = 0x02,
        Lwrom = 0x03,
        Lwromo = 0x04,
        Lbram = 0x05,
        Lbramo = 0x06,
        Lwram = 0x07,
        Lwramo = 0x08,
        Popb = 0x09,
        Popw = 0x0A,
        Lbext = 0x0B,

        #endregion

        #region Store
        Sbram = 0x0C,
        SbramI = 0x0D,
        Sbramo = 0x0E,
        Swram = 0x0F,
        Swramo = 0x10,
        Pushb = 0x11,
        Pushw = 0x12,
        Sbext = 0x13,

        #endregion

        #region Math
        Add = 0x14,
        AddI = 0x15,
        Sub = 0x16,
        SubI = 0x17,
        Addw = 0x18,
        Negw = 0x19,

        #endregion

        #region Logic
        Not = 0x1A,
        Or = 0x1B,
        And = 0x1C,
        Xor = 0x1D,
        Sll = 0x1E,
        Srl = 0x1F,

        #endregion

        #region Regs
        Set = 0x20,
        SetI = 0x21,
        Seth = 0x22,
        SethI = 0x23,
        Setw = 0x24,
        Setloh = 0x25,
        Swap = 0x26,
        Swaph = 0x27,
        Swapw = 0x28,
        Swaploh = 0x29,

        #endregion

        #region Jumps procedural
        Jr = 0x2A,
        Jro = 0x2B,
        Jas = 0x2C,
        JpcaddI = 0x2D,
        JpcsubI = 0x2E,

        #endregion

        #region Interrupts
        Irrex = 0x34,
        Irrret = 0x35,
        Irren = 0x36,
        Irrdis = 0x37,

        #endregion

        #region Jumps conditional
        Jwz = 0x38,
        Jwnotz = 0x39,
        Jwn = 0x3A,
        Jwnotn = 0x3B,
        Jzf = 0x3C,
        Jnf = 0x3D,
        Jcf = 0x3E,
        Jof = 0x3F,

        #endregion
    }
}

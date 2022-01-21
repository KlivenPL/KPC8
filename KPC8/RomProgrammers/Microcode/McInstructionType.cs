using KPC8._Infrastructure.Microcode.Attributes;

namespace KPC8.RomProgrammers.Microcode {
    public enum McInstructionType : ushort {
        [McInstructionName("Nop")]
        Nop = 0x0,

        #region Load

        [McInstructionName("Lbrom")]
        Lbrom = 0x01,

        [McInstructionName("Lbromo")]
        Lbromo = 0x02,

        [McInstructionName("Lwrom")]
        Lwrom = 0x03,

        [McInstructionName("Lwromo")]
        Lwromo = 0x04,

        [McInstructionName("Lbram")]
        Lbram = 0x05,

        [McInstructionName("Lbramo")]
        Lbramo = 0x06,

        [McInstructionName("Lwram")]
        Lwram = 0x07,

        [McInstructionName("Lwramo")]
        Lwramo = 0x08,

        [McInstructionName("Popb")]
        Popb = 0x09,

        [McInstructionName("Popw")]
        Popw = 0x0A,

        [McInstructionName("Lbext")]
        Lbext = 0x0B,

        #endregion

        #region Store

        [McInstructionName("Sbram")]
        Sbram = 0x0C,

        [McInstructionName("SbramI")]
        SbramI = 0x0D,

        [McInstructionName("Sbramo")]
        Sbramo = 0x0E,

        [McInstructionName("Swram")]
        Swram = 0x0F,

        [McInstructionName("Swramo")]
        Swramo = 0x10,

        [McInstructionName("Pushb")]
        Pushb = 0x11,

        [McInstructionName("Pushw")]
        Pushw = 0x12,

        [McInstructionName("Sbext")]
        Sbext = 0x13,

        #endregion

        #region Math

        [McInstructionName("Add")]
        Add = 0x14,

        [McInstructionName("AddI")]
        AddI = 0x15,

        [McInstructionName("Sub")]
        Sub = 0x16,

        [McInstructionName("SubI")]
        SubI = 0x17,

        [McInstructionName("Addw")]
        Addw = 0x18,

        [McInstructionName("Negw")]
        Negw = 0x19,

        #endregion

        #region Logic

        [McInstructionName("Not")]
        Not = 0x1A,

        [McInstructionName("Or")]
        Or = 0x1B,

        [McInstructionName("And")]
        And = 0x1C,

        [McInstructionName("Xor")]
        Xor = 0x1D,

        [McInstructionName("Sll")]
        Sll = 0x1E,

        [McInstructionName("Srl")]
        Srl = 0x1F,

        #endregion

        #region Regs

        [McInstructionName("Set")]
        Set = 0x20,

        [McInstructionName("SetI")]
        SetI = 0x21,

        [McInstructionName("Seth")]
        Seth = 0x22,

        [McInstructionName("SethI")]
        SethI = 0x23,

        [McInstructionName("Setw")]
        Setw = 0x24,

        [McInstructionName("Setloh")]
        Setloh = 0x25,

        [McInstructionName("Swap")]
        Swap = 0x26,

        [McInstructionName("Swaph")]
        Swaph = 0x27,

        [McInstructionName("Swapw")]
        Swapw = 0x28,

        [McInstructionName("Swaploh")]
        Swaploh = 0x29,

        #endregion

        #region Jumps procedural

        [McInstructionName("Jr")]
        Jr = 0x2A,

        [McInstructionName("Jro")]
        Jro = 0x2B,

        [McInstructionName("Jas")]
        Jas = 0x2C,

        [McInstructionName("JpcaddI")]
        JpcaddI = 0x2D,

        [McInstructionName("JpcsubI")]
        JpcsubI = 0x2E,

        #endregion

        #region Interrupts

        [McInstructionName("Irrex")]
        Irrex = 0x34,

        [McInstructionName("Irrret")]
        Irrret = 0x35,

        [McInstructionName("Irren")]
        Irren = 0x36,

        [McInstructionName("Irrdis")]
        Irrdis = 0x37,

        #endregion

        #region Jumps conditional

        [McInstructionName("Jwz")]
        Jwz = 0x38,

        [McInstructionName("Jwnotz")]
        Jwnotz = 0x39,

        [McInstructionName("Jwn")]
        Jwn = 0x3A,

        [McInstructionName("Jwnotn")]
        Jwnotn = 0x3B,

        [McInstructionName("Jzf")]
        Jzf = 0x3C,

        [McInstructionName("Jnf")]
        Jnf = 0x3D,

        [McInstructionName("Jcf")]
        Jcf = 0x3E,

        [McInstructionName("Jof")]
        Jof = 0x3F,

        #endregion
    }
}

using KPC8._Infrastructure.Microcode.Attributes;

namespace KPC8.RomProgrammers.Microcode {
    public enum McInstructionType : ushort {
        [McInstructionName("Nop")]
        Nop = 0x0,

        #region Math

        [McInstructionName("Add")]
        Add = 0x10,

        [McInstructionName("AddI")]
        AddI = 0x11,

        [McInstructionName("Sub")]
        Sub = 0x12,

        [McInstructionName("SubI")]
        SubI = 0x13,

        [McInstructionName("AddW")]
        AddW = 0x14,

        [McInstructionName("NegW")]
        NegW = 0x15,

        #endregion

        #region Logic

        [McInstructionName("Not")]
        Not = 0x16,

        [McInstructionName("Or")]
        Or = 0x17,

        [McInstructionName("And")]
        And = 0x18,

        [McInstructionName("Xor")]
        Xor = 0x19,

        [McInstructionName("Sll")]
        Sll = 0x1A,

        [McInstructionName("Srl")]
        Srl = 0x1B,

        #endregion

        /*[McInstructionDevName("Mov")]
        Mov = 0x16,

        [McInstructionDevName("MovI")]
        MovI = 0x17,*/
    }
}

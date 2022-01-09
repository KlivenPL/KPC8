using KPC8._Infrastructure.Microcode.Attributes;

namespace KPC8.RomProgrammers.Microcode {
    public enum McInstructionType : ushort {
        [McInstructionDevName("Nop")]
        Nop = 0x0,

        [McInstructionDevName("Add")]
        Add = 0x10,

        [McInstructionDevName("AddI")]
        AddI = 0x11,

        [McInstructionDevName("Sub")]
        Sub = 0x12,

        [McInstructionDevName("SubI")]
        SubI = 0x13,

        [McInstructionDevName("AddC")]
        AddC = 0x14,

        [McInstructionDevName("AddIC")]
        AddIC = 0x15,

        /*[McInstructionDevName("Mov")]
        Mov = 0x16,

        [McInstructionDevName("MovI")]
        MovI = 0x17,*/
    }
}

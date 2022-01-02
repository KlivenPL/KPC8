using KPC8._Infrastructure.Microcode.Attributes;

namespace KPC8.RomProgrammers.Microcode {
    public enum McInstructionType : ushort {
        [McInstructionDevName("Nop")]
        Nop = 0x0,

        [McInstructionDevName("Add")]
        Add = 0x10,

        [McInstructionDevName("AddI")]
        AddI = 0x11,
    }
}

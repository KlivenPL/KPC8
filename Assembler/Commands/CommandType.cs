namespace Assembler.Commands {
    public enum CommandType {
        None,
        SetAddress,
        Reserve,
        Ascii,
        Asciiz,
        Defnum,
        Defreg,
        DefcolorRGB,
        DefcolorHEX,
        Binfile,
        DebugWrite,
    }
}

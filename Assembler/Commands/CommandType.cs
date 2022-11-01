namespace Assembler.Commands {
    public enum CommandType {
        None,
        SetAddress,
        SetModuleAddress,
        Reserve,
        Ascii,
        Asciiz,
        Defnum,
        Defreg,
        DefcolorRGB,
        DefcolorHEX,
        Binfile,
        DebugWrite,
        InsertModule,
        ExportRegion,
    }
}

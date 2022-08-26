namespace Assembler.Pseudoinstructions {
    enum PseudoinstructionType {
        LbromI,
        LbromoI,
        LwromI,
        LwromoI,
        LbramI,
        LbramoI,
        LwramI,
        LwramoI,
        LbextI,

        SbramI,
        SbramoI,
        SwramI,
        SwramoI,
        SbextI,

        AddwI,

        Notw,
        OrI,
        Orw,
        OrwI,
        AndI,
        Andw,
        AndwI,
        XorI,
        Xorw,
        XorwI,

        SetwI,
        Getl,

        Jl,
        Jasl,

        Jwzl,
        Jwnotzl,
        Jwnl,
        Jwnotnl,
        Jzfl,
        Jnfl,
        Jcfl,
        Jofl,
    }
}

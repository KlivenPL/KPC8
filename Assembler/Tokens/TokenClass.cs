using System;

namespace Assembler.Tokens {
    [Flags]
    public enum TokenClass : uint {
        None = 0,
        Identifier = 1,      // mov SUBI AdDW label
        Register = 1 << 1,  // $t1, $t2, $0, $15
        Number = 1 << 2,    // 0, 1, -2, 2137
        Char = 1 << 3,      // 'a', 'b', '\0', '\t', '''
        String = 1 << 4,    // "abc", "a b c d\0", "ala ma KoTa"
        Label = 1 << 5,     // :loop, :abc
        Region = 1 << 6,    // *reg1, *getInput
        Command = 1 << 7,   // .address, .asciiz
    }
}

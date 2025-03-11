using Abstract.Components;

namespace Abstract {
    public interface IKpcBuild {
        IRegister16[] ProgrammerRegisters { get; }
        IMemory Rom { get; }
        IMemory Ram { get; }
        IRegister16 Pc { get; }
        IRegister16 Mar { get; }
        IRegister4 Flags { get; }
    }
}

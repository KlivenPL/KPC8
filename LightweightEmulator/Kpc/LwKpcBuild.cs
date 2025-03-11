using Abstract;
using Abstract.Components;
using LightweightEmulator.Components;
using LightweightEmulator.ExternalDevices;
using LightweightEmulator.Pipelines;

namespace LightweightEmulator.Kpc {
    public class LwKpcBuild : IKpcBuild {
        public LwKpcBuild(byte[]? rom, byte[]? ram,
            LwExternalDevicesAdapter extDeviceAdapter,
            LwInterruptsManager irrManager) {
            ProgrammerRegisters = Enumerable.Range(0, 16)
                .Select(x => new Register16()).ToArray();
            ProgrammerRegisters[0] = new ZeroRegister();

            Rom = new Memory(ushort.MaxValue + 1, rom);
            Ram = new Memory(ushort.MaxValue + 1, ram);
            Pc = new();
            Mar = new();
            Flags = new();
            ExtDeviceAdapter = extDeviceAdapter;
            IrrManager = irrManager;
        }

        public Register16[] ProgrammerRegisters { get; }
        public Memory Rom { get; }
        public Memory Ram { get; }

        public Register16 Pc { get; }
        public Register16 Mar { get; }

        public Register4 Flags { get; }

        public LwExternalDevicesAdapter ExtDeviceAdapter { get; }
        public LwInterruptsManager IrrManager { get; }

        IRegister16[] IKpcBuild.ProgrammerRegisters => ProgrammerRegisters;
        IMemory IKpcBuild.Rom => Rom;
        IMemory IKpcBuild.Ram => Ram;
        IRegister16 IKpcBuild.Pc => Pc;
        IRegister16 IKpcBuild.Mar => Mar;
        IRegister4 IKpcBuild.Flags => Flags;
    }
}

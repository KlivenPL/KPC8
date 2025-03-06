using Infrastructure.BitArrays;
using KPC8.ControlSignals;
using KPC8.CpuFlags;
using KPC8.ProgRegs;
using LightweightEmulator.Kpc;
using System.Linq;

namespace Tests._Infrastructure {
    public static class EmuLwIntegrity {
        public static void AssertFullIntegrity(LwKpcBuild lwBuild, ModulePanel emuModulePanel) {
            AssertPcIntegrity(lwBuild, emuModulePanel);
            AssertMarIntegrity(lwBuild, emuModulePanel);
            AssertFlagsIntegrity(lwBuild, emuModulePanel);
            AssertRegsIntegrity(lwBuild, emuModulePanel);
            AssertRomIntegrity(lwBuild, emuModulePanel);
            AssertRamIntegrity(lwBuild, emuModulePanel);
        }

        public static void AssertPcIntegrity(LwKpcBuild lwBuild, ModulePanel emuModulePanel) {
            var emu = emuModulePanel.Memory.PcContent;
            var lw = BitArrayHelper.FromUShortLE(lwBuild.Pc.WordValue);
            BitAssert.Equality(emu, lw, $"[EMU/LW {nameof(AssertPcIntegrity)}]:\n");
        }

        public static void AssertMarIntegrity(LwKpcBuild lwBuild, ModulePanel emuModulePanel) {
            var emu = emuModulePanel.Memory.MarContent;
            var lw = BitArrayHelper.FromUShortLE(lwBuild.Mar.WordValue);
            BitAssert.Equality(emu, lw, $"[EMU/LW {nameof(AssertMarIntegrity)}]:\n");
        }

        public static void AssertRegsIntegrity(LwKpcBuild lwBuild, ModulePanel emuModulePanel) {
            foreach (var regType in System.Enum.GetValues<Regs>().Except(new[] { Regs.None })) {
                var lw = BitArrayHelper.FromUShortLE(lwBuild.ProgrammerRegisters[regType.GetIndex()].WordValue);
                var emu = emuModulePanel.Registers.GetWholeRegContent(regType.GetIndex());

                BitAssert.Equality(emu, lw, $"[EMU/LW {nameof(AssertRegsIntegrity)}] {System.Enum.GetName(regType)}:\n");
            }
        }

        public static void AssertFlagsIntegrity(LwKpcBuild lwBuild, ModulePanel emuModulePanel) {
            var emu = emuModulePanel.Alu.RegFlagsContent;
            var lw = BitArrayHelper.FromByteLE(lwBuild.Flags.Value);
            BitAssert.Equality(emu, lw.Skip(4), $"[EMU/LW {nameof(AssertFlagsIntegrity)}]:" +
                $"\nEMU flags: {CpuFlagExtensions.From8BitArray(BitArrayHelper.FromString("0000").MergeWith(emu))}\nLW flags: {CpuFlagExtensions.From8BitArray(lw)}\n");
        }

        public static void AssertRomIntegrity(LwKpcBuild lwBuild, ModulePanel emuModulePanel) {
            var emu = emuModulePanel.Memory.RomDumpToBytesLE();
            var lw = lwBuild.Rom.DumpToBytes();
            BitAssert.Equality(emu, lw, $"[EMU/LW {nameof(AssertRomIntegrity)}]:\n");
        }

        public static void AssertRamIntegrity(LwKpcBuild lwBuild, ModulePanel emuModulePanel) {
            var emu = emuModulePanel.Memory.RamDumpToBytesLE();
            var lw = lwBuild.Ram.DumpToBytes();
            BitAssert.Equality(emu, lw, $"[EMU/LW {nameof(AssertRamIntegrity)}]:\n");
        }
    }
}

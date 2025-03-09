using Infrastructure.BitArrays;
using KPC8.ControlSignals;
using KPC8.CpuFlags;
using KPC8.ProgRegs;
using LightweightEmulator.Kpc;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Sdk; // for XunitException

namespace Tests._Infrastructure {
    public static class EmuLwIntegrity {
        public static void AssertFullIntegrity(LwKpcBuild lwBuild, ModulePanel emuModulePanel) {
            var errorMessages = new List<string>();

            try {
                AssertPcIntegrity(lwBuild, emuModulePanel);
            } catch (Exception ex) {
                errorMessages.Add(ex.Message);
            }
            try {
                AssertMarIntegrity(lwBuild, emuModulePanel);
            } catch (Exception ex) {
                errorMessages.Add(ex.Message);
            }
            try {
                AssertFlagsIntegrity(lwBuild, emuModulePanel);
            } catch (Exception ex) {
                errorMessages.Add(ex.Message);
            }
            try {
                AssertIrrIntegrity(lwBuild, emuModulePanel);
            } catch (Exception ex) {
                errorMessages.Add(ex.Message);
            }
            try {
                AssertRegsIntegrity(lwBuild, emuModulePanel);
            } catch (Exception ex) {
                errorMessages.Add(ex.Message);
            }
            try {
                AssertRomIntegrity(lwBuild, emuModulePanel);
            } catch (Exception ex) {
                errorMessages.Add(ex.Message);
            }
            try {
                AssertRamIntegrity(lwBuild, emuModulePanel);
            } catch (Exception ex) {
                errorMessages.Add(ex.Message);
            }

            if (errorMessages.Any()) {
                var combinedMessage = string.Join(Environment.NewLine, errorMessages);
                throw new XunitException(combinedMessage);
            }
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
            foreach (var regType in Enum.GetValues<Regs>().Except(new[] { Regs.None })) {
                var lw = BitArrayHelper.FromUShortLE(lwBuild.ProgrammerRegisters[regType.GetIndex()].WordValue);
                var emu = emuModulePanel.Registers.GetWholeRegContent(regType.GetIndex());
                BitAssert.Equality(emu, lw, $"[EMU/LW {nameof(AssertRegsIntegrity)}] {Enum.GetName(regType)}:\n");
            }
        }

        public static void AssertFlagsIntegrity(LwKpcBuild lwBuild, ModulePanel emuModulePanel) {
            var emu = emuModulePanel.Alu.RegFlagsContent;
            var lw = BitArrayHelper.FromByteLE(lwBuild.Flags.Value);
            BitAssert.Equality(emu, lw.Skip(4), $"[EMU/LW {nameof(AssertFlagsIntegrity)}]:\n" +
                $"EMU flags: {CpuFlagExtensions.From8BitArray(BitArrayHelper.FromString("0000").MergeWith(emu))}\n" +
                $"LW flags: {CpuFlagExtensions.From8BitArray(lw)}\n");
        }

        public static void AssertIrrIntegrity(LwKpcBuild lwBuild, ModulePanel emuModulePanel) {
            bool emuEn = emuModulePanel.InterruptsBus.Lanes[2];
            // bool emuBusy = emuModulePanel.InterruptsBus.Lanes[3];
            bool lwEn = lwBuild.IrrManager.En;
            // bool lwBusy = lwBuild.IrrManager.Busy;

            if (emuEn != lwEn) {
                throw new Exception($"[EMU/LW {nameof(AssertIrrIntegrity)}]:\n" +
                    $"Expected EN:\t{emuEn}\n" +
                    $"Actual   EN:\t{lwEn}");
            }
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

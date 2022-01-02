using Infrastructure.BitArrays;
using KPC8.ControlSignals;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections;
using Tests._Infrastructure;
using Xunit;
using Cs = KPC8.ControlSignals.ControlSignalType;

namespace Tests.KPC8Tests.Microcode.Instructions {
    public class AddProceduralInstructionTests : TestBase {

        [Fact]
        public void AddInstructionTests() {
            var opCode = McInstructionType.Add.Get6BitsOPCode();
            var addInstructionHigh = BitArrayHelper.FromString($"{opCode.ToBitString()}{Regs.T1.GetEncodedAddress().Skip(2).ToBitString()}");

            var addInstructionLow = BitArrayHelper.FromString($"{Regs.S1.GetEncodedAddress().ToBitString()}{Regs.S2.GetEncodedAddress().ToBitString()}");

            // var pc = BuildPcModules();
        }

        [Fact]
        public void AddIInstructionTests() {
            var opCode = McInstructionType.AddI.Get6BitsOPCode();
            var addInstructionHigh = BitArrayHelper.FromString($"{opCode.ToBitString()}{Regs.T2.GetEncodedAddress().Skip(2).ToBitString()}");
            var addInstructionLow = BitArrayHelper.FromString($"00101100");
            var totalInstruction = addInstructionHigh.MergeWith(addInstructionLow);
            var romData = new[] {
                addInstructionHigh,
                addInstructionLow,
            };

            var cp = BuildPcModules(romData, out var modules);


            _testSimulationLoop.Loop();
            _testSimulationLoop.Loop();
            _testSimulationLoop.Loop();
            _testSimulationLoop.Loop();

            BitAssert.Equality((Cs.Pc_oe | Cs.Mar_le_hi | Cs.Mar_le_lo).ToBitArray(), modules.ControlBus.Lanes);
            Assert.True(cp.Mem.Pc_oe);
            Assert.True(cp.Mem.Mar_le_hi);
            Assert.True(cp.Mem.Mar_le_lo);

            MakeTickAndWait();
            BitAssert.Equality((Cs.Pc_ce | Cs.Rom_oe | Cs.Ir_le_hi).ToBitArray(), modules.ControlBus.Lanes);
            Assert.True(cp.Mem.Pc_ce);
            Assert.True(cp.Mem.Rom_oe);
            Assert.True(cp.Ctrl.Ir_le_hi);


            MakeTickAndWait();
            BitAssert.Equality((Cs.Pc_ce | Cs.Mar_ce | Cs.Rom_oe | Cs.Ir_le_lo).ToBitArray(), modules.ControlBus.Lanes);
            MakeTickAndWait();
            MakeTickAndWait();
            MakeTickAndWait();
            MakeTickAndWait();
            MakeTickAndWait();
            MakeTickAndWait();
            MakeTickAndWait();
            MakeTickAndWait();
            MakeTickAndWait();
            MakeTickAndWait();


            /*            MakeTickAndWait();


                        MakeTickAndWait();
                        MakeTickAndWait();
                        //            BitAssert.Equality(totalInstruction, modules.Control.IrContent);*/

            BitAssert.Equality(addInstructionLow, modules.Registers.GetRegContent(Regs.T2.GetIndex()));
        }

        private CsPanel BuildPcModules(BitArray[] romData, out ModulePanel modules) {
            var cp = new CpuBuilder(_testClock)
               .WithControlModule(null, true)
               .WithMemoryModule(romData, null)
               .WithRegistersModule()
               .WithAluModule()
               .BuildWithModulesAccess(out modules);

            return cp;
        }
    }
}

using _Infrastructure.BitArrays;
using Autofac.Features.AttributeFilters;
using Components.Adders;
using Components.Buses;
using Components.Clocks;
using Components.Counters;
using Components.Logic;
using Components.Rams;
using Components.Registers;
using Components.Roms;
using Components.Signals;
using Components.Transcievers;
using Infrastructure.BitArrays;
using KPC8.Clocks;
using Simulation.Loops;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KPC8 {
    class Application {

        private readonly Clock mainClock;
        private readonly SimulationLoop loop;

        private Signal mainClockBar;
        private Inverter mainClockInverter;
        private HLBus dataBus;
        private HLBus addressBus;
        private HLRegister regA;
        private HLRegister regB;
        private HLCounter mar;
        private HLRegister ir;
        private HLAdder aluAdder;
        private HLCounter pc;
        private HLCounter ic;
        private HLRam ram;
        private HLRom rom;
        private HLTransciever transcAtoBus;
        private HLTransciever transcBtoBus;
        private int cycleNumber = 0;

        private Signal regALoadEnable;
        private Signal.Readonly regAOutputEnableReadonly;
        private Signal regAClear;
        private Signal regAtoBusEnable;
        private Signal regBLoadEnable;
        private Signal.Readonly regBOutputEnableReadonly;
        private Signal regBClear;
        private Signal regBtoBusEnable;
        private Signal aluOutputEnable;
        private Signal aluSubstractEnable;
        private Signal aluCarryIn;
        private Signal aluCarryOut;
        private Signal pcClear;
        private Signal pcCountEnable;
        private Signal pcLoadEnable;
        private Signal pcOutputEnable;
        private Signal marLoadEnable;
        private Signal ramOutputEnable;
        private Signal ramWriteEnable;

        private readonly string[] initialMemory = new string[] {
            "00001110", // 0
            "00001111", // 1 
            "00000000", // 2
            "00000000", // 3
            "00000000", // 4
            "00000000", // 5
            "00000000", // 6
            "00000000", // 7
            "00000000", // 8
            "00000000", // 9
            "00000000", // 10
            "00000000", // 11
            "00000000", // 12
            "00000000", // 13
            "00010101", // 14
            "00100101", // 15
        };

        public Application(
            [KeyFilter(ClockType.MainManualClock)] Clock mainClock,
            SimulationLoop loop) {

            this.mainClock = mainClock;
            this.loop = loop;

            Create(initialMemory.Select(s => BitArrayHelper.FromString(s)).ToArray());
        }

        public void Run() {
            /*while (true) {
                Enable(pcOutputEnable);
                Enable(marLoadEnable);

                MakeTickAndWait();
                if (ShouldEscape()) break;

                Enable(pcCountEnable);
                Enable(ramOutputEnable);
                Enable(regALoadEnable);

                MakeTickAndWait();
                if (ShouldEscape()) break;

                Enable(pcOutputEnable);
                Enable(marLoadEnable);

                MakeTickAndWait();
                if (ShouldEscape()) break;

                Enable(ramOutputEnable);
                Enable(regBLoadEnable);

                MakeTickAndWait();
                if (ShouldEscape()) break;

                Enable(aluOutputEnable);
                Enable(regALoadEnable);

                MakeTickAndWait();
                if (ShouldEscape()) break;
            }*/

            while (true) {

                MakeTickAndWait();
                if (ShouldEscape()) break;
            }
        }


        private List<Signal> cycleSignals = new List<Signal>();
        private void Enable(Signal signal) {
            signal.Value = true;
            cycleSignals.Add(signal);
        }

        private void Create(BitArray[] initialMemory = null) {

            mainClockInverter = new Inverter(1);
            mainClockInverter.Inputs[0].PlugIn(mainClock.Clk);
            mainClockBar = Signal.Factory.Create(nameof(mainClockBar));
            mainClockInverter.Outputs[0].PlugIn(mainClockBar);

            #region 8Bit

            dataBus = new HLBus("dataBus", 8);
            regA = new HLRegister(8);
            regB = new HLRegister(8);
            ic = new HLCounter(8);
            aluAdder = new HLAdder(8);
            transcAtoBus = new HLTransciever(8);
            transcBtoBus = new HLTransciever(8);

            #endregion

            #region 16Bit

            addressBus = new HLBus("addressBus", 16);
            pc = new HLCounter(16);
            ram = new HLRam(8, 16, (int)Math.Pow(2, 16), initialMemory);
            rom = new HLRom(8, 16, (int)Math.Pow(2, 16), null);
            mar = new HLCounter(16);
            ir = new HLRegister(16);

            #endregion

            dataBus
                .Connect(0, 8, regA.Inputs.Skip(0))
                .Connect(0, 8, transcAtoBus.Outputs.Skip(0))
                .Connect(0, 8, regB.Inputs.Skip(0))
                .Connect(0, 8, transcBtoBus.Outputs.Skip(0))
                .Connect(0, 8, aluAdder.Outputs.Skip(0))
                .Connect(0, 8, pc.Inputs.Skip(8))
                .Connect(0, 8, mar.Inputs.Skip(8))
                .Connect(0, 8, ram.DataInputs.Skip(0))
                .Connect(0, 8, ram.Outputs.Skip(0))
                .Connect(0, 8, ir.Inputs.Skip(8))
                .Connect(0, 8, ir.Outputs.Skip(8));

            addressBus
                .Connect(0, 16, pc.Outputs.Skip(0))
                .Connect(0, 16, mar.Outputs.Skip(0))
                .Connect(0, 16, ram.AddressInputs.Skip(0))
                .Connect(0, 16, rom.AddressInputs.Skip(0));

            regA.Clk.PlugIn(mainClock.Clk);
            regB.Clk.PlugIn(mainClock.Clk);
            pc.Clk.PlugIn(mainClock.Clk);
            mar.Clk.PlugIn(mainClock.Clk);
            ir.Clk.PlugIn(mainClock.Clk);
            ic.Clk.PlugIn(mainClockBar);

            Signal.Factory.CreateAndConnectPorts("RegAOutputsToAluAdderInputs", regA.Outputs, aluAdder.Inputs.Take(8));
            Signal.Factory.CreateAndConnectPorts("RegBOutputsToAluAdderInputs", regB.Outputs, aluAdder.Inputs.Skip(8));

            Signal.Factory.CreateAndConnectPorts("RegAOutputsToTranscAtoBusInputs", regA.Outputs, transcAtoBus.Inputs.Skip(0));
            Signal.Factory.CreateAndConnectPorts("RegBOutputsToTranscBtoBusInputs", regB.Outputs, transcBtoBus.Inputs.Skip(0));

            Signal.Factory.CreateAndConnectPorts("RegBOutputsToTranscBtoBusInputs", regB.Outputs, transcBtoBus.Inputs.Skip(0));

            regALoadEnable = regA.CreateSignalAndPlugin(nameof(regA), x => x.LoadEnable);
            var regAOutputEnable = regA.CreateSignalAndPlugin(nameof(regA), x => x.OutputEnable);
            regAClear = regA.CreateSignalAndPlugin(nameof(regA), x => x.Clear);
            regAtoBusEnable = transcAtoBus.CreateSignalAndPlugin(nameof(transcAtoBus), x => x.OutputEnable);

            regBLoadEnable = regB.CreateSignalAndPlugin(nameof(regB), x => x.LoadEnable);
            var regBOutputEnable = regB.CreateSignalAndPlugin(nameof(regB), x => x.OutputEnable);
            regBClear = regB.CreateSignalAndPlugin(nameof(regB), x => x.Clear);
            regBtoBusEnable = transcBtoBus.CreateSignalAndPlugin(nameof(transcBtoBus), x => x.OutputEnable);

            aluOutputEnable = aluAdder.CreateSignalAndPlugin(nameof(aluAdder), x => x.OutputEnable);
            aluSubstractEnable = aluAdder.CreateSignalAndPlugin(nameof(aluAdder), x => x.SubstractEnable);
            aluCarryIn = aluAdder.CreateSignalAndPlugin(nameof(aluAdder), x => x.CarryIn);
            aluCarryOut = aluAdder.CreateSignalAndPlugin(nameof(aluAdder), x => x.CarryOut);

            pcClear = pc.CreateSignalAndPlugin(nameof(pc), x => x.Clear);
            pcCountEnable = pc.CreateSignalAndPlugin(nameof(pc), x => x.CountEnable);
            pcLoadEnable = pc.CreateSignalAndPlugin(nameof(pc), x => x.LoadEnable);
            pcOutputEnable = pc.CreateSignalAndPlugin(nameof(pc), x => x.OutputEnable);

            var marOutputEnable = mar.CreateSignalAndPlugin(nameof(mar), x => x.OutputEnable);
            marLoadEnable = mar.CreateSignalAndPlugin(nameof(mar), x => x.LoadEnable);

            ramOutputEnable = ram.CreateSignalAndPlugin(nameof(ram), x => x.OutputEnable);
            ramWriteEnable = ram.CreateSignalAndPlugin(nameof(ram), x => x.WriteEnable);

            // const signals
            regAOutputEnable.Value = true;
            regAOutputEnableReadonly = regAOutputEnable;

            regBOutputEnable.Value = true;
            regBOutputEnableReadonly = regBOutputEnable;

            marOutputEnable.Value = true;
        }

        private void MakeTickAndWait() {
            loop.Loop();
            mainClock.MakeTick();
            while (mainClock.IsManualTickInProgress) {
                loop.Loop();
            }

            foreach (var sig in cycleSignals) {
                sig.Value = false;
            }

            cycleSignals.Clear();

            Console.WriteLine($"#{cycleNumber++}");
            Console.WriteLine($"PC:\t{pc.Content.ToBitStringWithDecAndHexLittleEndian()}");
            Console.WriteLine($"Bus:\t{dataBus.PeakAll().ToBitStringWithDecAndHexLittleEndian()}");
            Console.WriteLine($"MAR:\t{mar.Content.ToBitStringWithDecAndHexLittleEndian()} -> RAM: \t{ram.Content[mar.Content.ToIntLittleEndian()].ToBitStringWithDecAndHexLittleEndian()}");
            Console.WriteLine($"A:\t{regA.Content.ToBitStringWithDecAndHexLittleEndian()}");
            Console.WriteLine($"B:\t{regB.Content.ToBitStringWithDecAndHexLittleEndian()}");
            Console.WriteLine($"ALU:\t{aluAdder.Content.ToBitStringWithDecAndHexLittleEndian()}");
            Console.WriteLine();
        }

        private static bool ShouldEscape() {
            var key = Console.ReadKey();

            switch (key.Key) {
                case ConsoleKey.Enter:
                    break;
                case ConsoleKey.Escape:
                    return true;
            }

            return false;
        }
    }
}

﻿using Components.Signals;

namespace KPC8.ControlSignals {
    public interface ICsPanel { }
    public class CsPanel {
        public MemoryPanel Mem { get; init; }
        public ControlPanel Ctrl { get; init; }
        public RegsPanel Regs { get; init; }
        public AluPanel Alu { get; init; }

        public class MemoryPanel : ICsPanel {
            [ControlSignal(ControlSignalType.Pc_le_hi)]
            public Signal Pc_le_hi { get; init; }

            [ControlSignal(ControlSignalType.Pc_le_lo)]
            public Signal Pc_le_lo { get; init; }

            [ControlSignal(ControlSignalType.Pc_oe)]
            public Signal Pc_oe { get; init; }

            [ControlSignal(ControlSignalType.Pc_ce)]
            public Signal Pc_ce { get; init; }

            [ControlSignal(ControlSignalType.Mar_le_hi)]
            public Signal Mar_le_hi { get; init; }

            [ControlSignal(ControlSignalType.Mar_le_lo)]
            public Signal Mar_le_lo { get; init; }

            [ControlSignal(ControlSignalType.MarToBus_oe)]
            public Signal MarToBus_oe { get; init; }

            [ControlSignal(ControlSignalType.Mar_ce)]
            public Signal Mar_ce { get; init; }

            [ControlSignal(ControlSignalType.Ram_we)]
            public Signal Ram_we { get; init; }

            [ControlSignal(ControlSignalType.Ram_oe)]
            public Signal Ram_oe { get; init; }

            [ControlSignal(ControlSignalType.Rom_oe)]
            public Signal Rom_oe { get; init; }
        }

        public class ControlPanel : ICsPanel {
            [ControlSignal(ControlSignalType.Ir_le_hi)]
            public Signal Ir_le_hi { get; init; }

            [ControlSignal(ControlSignalType.Ir_le_lo)]
            public Signal Ir_le_lo { get; init; }

            [ControlSignal(ControlSignalType.Ir8LSBToBus_oe)]
            public Signal Ir8LSBToBus_oe { get; init; }

            [ControlSignal(ControlSignalType.Ic_clr)]
            public Signal Ic_clr { get; init; }

            [ControlSignal(ControlSignalType.DecDest_oe)]
            public Signal DecDest_oe { get; init; }

            [ControlSignal(ControlSignalType.DecA_oe)]
            public Signal DecA_oe { get; init; }

            [ControlSignal(ControlSignalType.DecB_oe)]
            public Signal DecB_oe { get; init; }
        }

        public class RegsPanel : ICsPanel {
            [ControlSignal(ControlSignalType.Regs_le)]
            public Signal Regs_le { get; init; }

            [ControlSignal(ControlSignalType.Regs_oe)]
            public Signal Regs_oe { get; init; }
        }

        public class AluPanel : ICsPanel {
            [ControlSignal(ControlSignalType.RegA_le)]
            public Signal RegA_le { get; init; }

            [ControlSignal(ControlSignalType.RegAToBus_oe)]
            public Signal RegAToBus_oe { get; init; }

            [ControlSignal(ControlSignalType.RegB_le)]
            public Signal RegB_le { get; init; }

            [ControlSignal(ControlSignalType.RegBToBus_oe)]
            public Signal RegBToBus_oe { get; init; }

            [ControlSignal(ControlSignalType.Alu_oe)]
            public Signal Alu_oe { get; init; }

            [ControlSignal(ControlSignalType.Alu_sube)]
            public Signal Alu_sube { get; init; }
        }
    }
}

using System;

namespace KPC8.ControlSignals {
    public class ControlSignalsPanelBuilder {
        private Func<CsPanel.MemoryPanel> createMemoryPanel;
        private Func<CsPanel.ControlPanel> createControlPanel;
        private Func<CsPanel.RegsPanel> createRegsPanel;
        private Func<CsPanel.AluPanel> createAluPanel;

        public ControlSignalsPanelBuilder SetMemorySignals(Func<CsPanel.MemoryPanel> createMemoryPanel) {
            this.createMemoryPanel = createMemoryPanel;
            return this;
        }

        public ControlSignalsPanelBuilder SetControlSignals(Func<CsPanel.ControlPanel> createControlPanel) {
            this.createControlPanel = createControlPanel;
            return this;
        }

        public ControlSignalsPanelBuilder SetRegsSignals(Func<CsPanel.RegsPanel> createRegsPanel) {
            this.createRegsPanel = createRegsPanel;
            return this;
        }

        public ControlSignalsPanelBuilder SetAluSignals(Func<CsPanel.AluPanel> createAluPanel) {
            this.createAluPanel = createAluPanel;
            return this;
        }

        public CsPanel Build() {
            return new CsPanel {
                Mem = createMemoryPanel?.Invoke(),
                Ctrl = createControlPanel?.Invoke(),
                Regs = createRegsPanel?.Invoke(),
                Alu = createAluPanel?.Invoke()
            };
        }
    }
}

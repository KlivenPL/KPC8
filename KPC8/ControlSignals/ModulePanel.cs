using Components.Buses;
using KPC8.Modules;

namespace KPC8.ControlSignals {
    public class ModulePanel {
        public Memory Memory { get; set; }
        public Control Control { get; set; }
        public Registers Registers { get; set; }
        public Alu Alu { get; set; }

        public IBus DataBus { get; set; }
        public IBus AddressBus { get; set; }
        public IBus FlagsBus { get; set; }
        public IBus RegisterSelectBus { get; set; }
        public IBus ControlBus { get; set; }
        public IBus InterruptsBus { get; set; }
    }
}

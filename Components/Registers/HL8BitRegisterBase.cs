using Components.Signals;

namespace Components.Registers {
    abstract class HL8BitRegisterBase {
        public SignalPort Inp1 { get; protected set; } = new SignalPort();
        public SignalPort Inp2 { get; protected set; } = new SignalPort();
        public SignalPort Inp3 { get; protected set; } = new SignalPort();
        public SignalPort Inp4 { get; protected set; } = new SignalPort();
        public SignalPort Inp5 { get; protected set; } = new SignalPort();
        public SignalPort Inp6 { get; protected set; } = new SignalPort();
        public SignalPort Inp7 { get; protected set; } = new SignalPort();
        public SignalPort Inp8 { get; protected set; } = new SignalPort();
        public SignalPort[] Inputs { get; private set; }

        public SignalPort Out1 { get; protected set; } = new SignalPort();
        public SignalPort Out2 { get; protected set; } = new SignalPort();
        public SignalPort Out3 { get; protected set; } = new SignalPort();
        public SignalPort Out4 { get; protected set; } = new SignalPort();
        public SignalPort Out5 { get; protected set; } = new SignalPort();
        public SignalPort Out6 { get; protected set; } = new SignalPort();
        public SignalPort Out7 { get; protected set; } = new SignalPort();
        public SignalPort Out8 { get; protected set; } = new SignalPort();
        public SignalPort[] Outputs { get; private set; }

        public SignalPort Load { get; protected set; } = new SignalPort();
        public SignalPort Enable { get; protected set; } = new SignalPort();
        public SignalPort Clear { get; protected set; } = new SignalPort();
        public SignalPort Clk { get; protected set; } = new SignalPort();

        public virtual void Initialize() {
            Inputs = new[] {
                Inp1,
                Inp2,
                Inp3,
                Inp4,
                Inp5,
                Inp6,
                Inp7,
                Inp8,
            };

            Outputs = new[] {
                Out1,
                Out2,
                Out3,
                Out4,
                Out5,
                Out6,
                Out7,
                Out8,
            };
        }
    }
}

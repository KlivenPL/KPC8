using System;

namespace KPC8.ControlSignals {
    [AttributeUsage(AttributeTargets.Property)]
    class ControlSignalAttribute : Attribute {
        public ControlSignalAttribute(ControlSignalType controlSignalType) {
            ControlSignalType = controlSignalType;
        }

        public ControlSignalType ControlSignalType { get; }
    }
}

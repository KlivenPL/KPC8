using System;

namespace KPC8._Infrastructure.Microcode.Attributes {
    [AttributeUsage(AttributeTargets.Field)]
    public class McInstructionDevNameAttribute : Attribute {
        public string DevName { get; }

        public McInstructionDevNameAttribute(string devName) {
            DevName = devName.ToUpper();
        }
    }
}

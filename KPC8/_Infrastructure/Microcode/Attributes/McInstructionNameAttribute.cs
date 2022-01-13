using System;

namespace KPC8._Infrastructure.Microcode.Attributes {
    [AttributeUsage(AttributeTargets.Field)]
    public class McInstructionNameAttribute : Attribute {
        public string DevName { get; }

        public McInstructionNameAttribute(string devName) {
            DevName = devName.ToUpper();
        }
    }
}

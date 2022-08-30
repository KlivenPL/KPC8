namespace Assembler.Contexts.Labels {
    public class LabelInfo {
        public LabelInfo(string name, ushort? address) {
            Name = name;
            Address = address;
        }

        public string Name { get; set; }
        public ushort? Address { get; set; }
    }
}

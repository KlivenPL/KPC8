using LightweightEmulator.Components;

namespace LightweightEmulator.Kpc {
    internal class KpcBuild {
        public KpcBuild() {
            Registers = Enumerable.Range(0, 16).Select(x => new Register()).ToArray();
            Ram = new Memory(ushort.MaxValue + 1);
            Rom = new Memory(ushort.MaxValue + 1);
        }

        public Register[] Registers { get; }
        public Memory Ram { get; }
        public Memory Rom { get; }
    }
}

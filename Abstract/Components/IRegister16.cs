namespace Abstract.Components {
    public interface IRegister16 {
        byte HighValue { get; set; }
        byte LowValue { get; set; }
        ushort WordValue { get; set; }
    }
}

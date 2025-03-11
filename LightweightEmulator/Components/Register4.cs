using Abstract.Components;

namespace LightweightEmulator.Components {
    public class Register4 : IRegister4 {
        private byte _value;

        public byte Value {
            get => _value;
            set {
                _value = (byte)(0b00001111 & value);
            }
        }
    }
}

using System.Collections;

namespace Player.Controls.Register {
    internal class RegisterCtrlParameters {
        public RegisterCtrlParameters(string registerName, int registerSize, Func<BitArray> getWholeRegContent) {
            GetWholeRegContent = getWholeRegContent;
            RegisterName = registerName;
            RegisterSize = registerSize;
        }

        public Func<BitArray> GetWholeRegContent { get; }
        public string RegisterName { get; }
        public int RegisterSize { get; }
    }
}

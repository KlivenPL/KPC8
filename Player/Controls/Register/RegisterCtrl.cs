using Infrastructure.BitArrays;
using Player._Infrastructure;
using Player._Infrastructure.Events;
using Player.Events;
using System.Collections;

namespace Player.Controls.Register {
    internal partial class RegisterCtrl : UserControl, IInitializable<RegisterCtrlParameters>, IEventListener<UpdateRegistersGuiEvent> {
        private string registerName;
        private Func<BitArray> GetWholeRegContent;

        public RegisterCtrl() {
            InitializeComponent();
        }

        public void Initialize(RegisterCtrlParameters parameters) {
            registerName = parameters.RegisterName;
            regLbl.Text = registerName;
            SetContent(new BitArray(parameters.RegisterSize));

            this.ListenToEvent<UpdateRegistersGuiEvent>();
        }

        public void OnEvent(UpdateRegistersGuiEvent @event) {
            var content = GetWholeRegContent();
            SetContent(content);
        }

        private void SetContent(BitArray content) {
            ushort value = content.ToUShortLE();

            valueBinTxt.Text = BitArrayHelper.ToPrettyBitString(content);
            valueHexTxt.Text = $"0x{value:X4}";
            valueDecTxt.Text = value.ToString();
        }
    }
}

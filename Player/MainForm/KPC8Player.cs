using Infrastructure.BitArrays;
using KPC8.ProgRegs;
using Microsoft.Extensions.DependencyInjection;
using Player.Controls.Register;
using Player.Gui.Renderers;
using Player.GuiLogic.StateMachine;
using Player.InternalForms.BitArrayViewer;
using System.Collections;

namespace Player.MainForm {
    public partial class KPC8Player : Form {
        private readonly IServiceProvider provider;
        private readonly GuiStateManager guiStateManager;

        public KPC8Player(IServiceProvider provider) {
            InitializeComponent();
            this.provider = provider;
            this.guiStateManager = provider.GetRequiredService<GuiStateManager>();
            InitializeForm();
            InitializeRegisters();
        }

        private void InitializeForm() {
            mnuToolBar.Renderer = new CustomToolStripRenderer();
        }

        private void InitializeRegisters() {
            var registers = regsPnl.Controls.OfType<RegisterCtrl>().ToArray();
            var regsTypes = Enum.GetValues<Regs>();

            for (int i = 0; i < 16; i++) {
                registers[i].Initialize(new RegisterCtrlParameters(regsTypes[i + 1].ToString(), 16, () => new BitArray(16)));
            }
        }

        private void loadRomBtn_Click(object sender, EventArgs e) {
            var bavForm = provider.GetRequiredService<BitArrayViewerForm>();

            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Filter = "KPC8 ROM|*.kpcrom|All files|*.*";
            if (theDialog.ShowDialog() == DialogResult.OK) {
                bavForm.Show();
            }
        }

        private static BitArray[] LoadRomFromBinaryFile(string path) {
            var fileInfo = new FileInfo(path);
            BitArray[] bas = new BitArray[ushort.MaxValue + 1];
            using var stream = fileInfo.OpenRead();
            using var binaryReader = new BinaryReader(stream);

            for (int i = 0; i < ushort.MaxValue + 1; i++) {
                var @byte = binaryReader.ReadByte();
                bas[i] = BitArrayHelper.FromByteLE(@byte);
            }

            return bas;
        }

        private void KPC8Player_Load(object sender, EventArgs e) {
            guiStateManager.Initialize();

        }

        private void mnuPlayBtn_Click(object sender, EventArgs e) {
            guiStateManager.CurrentState.Play();
        }

        private void mnuStopBtn_Click(object sender, EventArgs e) {
            guiStateManager.CurrentState.Stop();
        }

        private void mnuDbgBtn_Click(object sender, EventArgs e) {
            guiStateManager.CurrentState.Debug();
        }

        /*
        var testData = LoadRomFromBinaryFile(theDialog.FileName);
                bavForm.Initialize(new Player.InternalForms.BitArrayViewer.BitArrayViewerFormParameters(
                    title: "Test title",
                    bitArraySource: () => testData,
                    Action
                    ));
        */



    }
}
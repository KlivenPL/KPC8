using KPC8.ProgRegs;
using Microsoft.Extensions.DependencyInjection;
using Player.Contexts;
using Player.Controls.Register;
using Player.Gui.Renderers;
using Player.GuiLogic.StateMachine;
using System.Collections;

namespace Player.MainForm {
    internal partial class KPC8Player : Form {
        private readonly IServiceProvider provider;
        private readonly GuiStateManager guiStateManager;
        private readonly ProgramContext programContext;

        private static KPC8Player instance;

        internal static void InvokeOnForm(Action method) => instance.Invoke(method);

        public KPC8Player(IServiceProvider provider, ProgramContext programContext) {
            InitializeComponent();
            this.provider = provider;
            this.guiStateManager = provider.GetRequiredService<GuiStateManager>();
            this.programContext = programContext;
            InitializeForm();
            InitializeRegisters();
            instance = this;
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

        private void loadRomBtn_Click(object sender, EventArgs e) {
            programContext.TryLoadRomFile();
        }

        private void mnuFileLoadSourceBtn_Click(object sender, EventArgs e) {
            programContext.TryLoadSourceFile();
        }

        /*var bavForm = provider.GetRequiredService<BitArrayViewerForm>();

            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Filter = "KPC8 ROM|*.kpcrom|All files|*.*";
            if (theDialog.ShowDialog() == DialogResult.OK) {
                bavForm.Show();
            }*/

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
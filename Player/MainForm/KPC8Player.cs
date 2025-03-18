using Microsoft.Extensions.DependencyInjection;
using OpenTK.GLControl;
using OpenTK.Windowing.Common;
using Player.Contexts;
using Player.Gui.Renderers;
using Player.GuiLogic.StateMachine;
using Runner._Infrastructure;

namespace Player.MainForm {
    internal partial class KPC8Player : Form {
        private readonly IServiceProvider provider;
        private readonly GuiStateManager guiStateManager;
        private readonly ProgramContext programContext;
        //private readonly RenderCanvas renderCanvas;
        private readonly GLControl renderCanvasGl;
        private IKPC8Renderer kpc8Renderer;

        private static KPC8Player instance;

        internal static void InvokeOnForm(Action method) => instance.Invoke(method);

        public KPC8Player(IServiceProvider provider, ProgramContext programContext) {
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
            //renderCanvas = CreateRenderCanvas();
            //canvasPnl.Controls.Add(renderCanvas);
            renderCanvasGl = CreateRenderCanvasGl();
            canvasPnl.Controls.Add(renderCanvasGl);

            this.provider = provider;
            this.guiStateManager = provider.GetRequiredService<GuiStateManager>();
            this.programContext = programContext;
            InitializeForm();
            instance = this;
        }

        private void InitializeForm() {
            mnuToolBar.Renderer = new CustomToolStripRenderer();
        }

        //private RenderCanvas CreateRenderCanvas() {
        //    var rc = new RenderCanvas(canvasPnl.Width, canvasPnl.Height, renderCanvasLock);
        //    Resize += (x, d) => rc.OnFormResize(canvasPnl.Width, canvasPnl.Height);
        //    OnResize(null);
        //    return rc;
        //}

        private GLControl CreateRenderCanvasGl() {
            var glSettings = new GLControlSettings {
                APIVersion = new Version(3, 3),
                Profile = ContextProfile.Compatability
            };

            var glControl = new GLControl(glSettings);
            glControl.Paint += GlControl_Paint;
            glControl.Load += GlControl_Load;
            return glControl;
        }

        private void GlControl_Load(object sender, EventArgs e) {
            renderCanvasGl.MakeCurrent();
            renderCanvasGl.BringToFront();
        }

        private void GlControl_Paint(object sender, PaintEventArgs e) {
            if (kpc8Renderer == null) {
                return;
            }

            if (kpc8Renderer.IsTextureReinitRequired()) {
                renderCanvasGl.Dock = DockStyle.Fill;
                kpc8Renderer.SetupTexture();
            }

            kpc8Renderer.UpdateTextureFromFrontBuffer();
            kpc8Renderer.RenderQuad(renderCanvasGl.Width, renderCanvasGl.Height);
            renderCanvasGl.SwapBuffers();
            renderCanvasGl.Invalidate();
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

        private void mnuPauseBtn_Click(object sender, EventArgs e) {
            guiStateManager.CurrentState.Pause();
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
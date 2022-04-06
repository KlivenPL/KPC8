namespace Player.MainForm {
    partial class KPC8Player {
        internal class Controller {
            private readonly KPC8Player form;

            public Controller(KPC8Player form) {
                this.form = form;
            }

            internal void FreezeFrom() {
                form.SuspendLayout();
            }

            internal void UnfreezeForm() {
                form.ResumeLayout();
            }

            internal ToolStrip mnuToolBar => form.mnuToolBar;
            internal ToolStripDropDownButton mnuFileDrop => form.mnuFileDrop;
            internal ToolStripMenuItem mnuFileLoadRomBtn => form.mnuFileLoadRomBtn;
            internal ToolStripButton mnuPlayBtn => form.mnuPlayBtn;
            internal ToolStripDropDownButton mnuDbgBtn => form.mnuDbgBtn;
            internal ToolStripMenuItem mnuDbgStepIntoBtn => form.mnuDbgStepIntoBtn;
            internal ToolStripMenuItem mnuDbgPlayBtn => form.mnuDbgPlayBtn;
            internal ToolStripButton mnuStopBtn => form.mnuStopBtn;
            internal ToolStripButton mnuPauseBtn => form.mnuPauseBtn;
            internal ToolStripButton mnuStepOverBtn => form.mnuStepOverBtn;
            internal ToolStripButton mnuStepIntoBtn => form.mnuStepIntoBtn;
            internal FlowLayoutPanel regsPnl => form.regsPnl;
        }
    }
}

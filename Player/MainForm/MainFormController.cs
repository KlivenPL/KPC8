using Player._Infrastructure.Controls;
using System.Text;

namespace Player.MainForm {
    partial class KPC8Player {
        internal class Controller {
            private const string MainTitle = "KPC8 Player";
            private readonly KPC8Player form;
            private string loadedFileName;
            private string statusTitle;

            public string LoadedFileName {
                get => loadedFileName;
                set {
                    loadedFileName = value;
                    RefreshTitle();
                }
            }

            public string StatusTitle {
                get => statusTitle;
                set {
                    statusTitle = value;
                    RefreshTitle();
                }
            }


            public Controller(KPC8Player form) {
                this.form = form;
            }

            internal void FreezeFrom() {
                mnuToolBar.OnUiThread(x => x.Enabled = false);
            }

            internal void UnfreezeForm() {
                mnuToolBar.OnUiThread(x => x.Enabled = true);
            }

            public void RefreshTitle() {
                var sb = new StringBuilder();
                sb.Append(MainTitle);
                if (loadedFileName != null) {
                    sb.Append($" - {loadedFileName}");
                }
                if (statusTitle != null) {
                    sb.Append($" ({statusTitle})");
                }

                form.Invoke(() => form.Text = sb.ToString());
            }

            internal ToolStrip mnuToolBar => form.mnuToolBar;
            internal ToolStripDropDownButton mnuFileDrop => form.mnuFileDrop;
            internal ToolStripMenuItem mnuFileLoadRomBtn => form.mnuFileLoadRomBtn;
            internal ToolStripButton mnuPlayBtn => form.mnuPlayBtn;
            internal ToolStripButton mnuDbgBtn => form.mnuDbgBtn;
            internal ToolStripButton mnuStopBtn => form.mnuStopBtn;
            internal ToolStripButton mnuPauseBtn => form.mnuPauseBtn;
            internal ToolStripButton mnuStepOverBtn => form.mnuStepOverBtn;
            internal ToolStripButton mnuStepIntoBtn => form.mnuStepIntoBtn;
            internal FlowLayoutPanel regsPnl => form.regsPnl;


            internal void TryLoadFile(string title, string filter, Action<FileInfo> selectedFileCallback) {
                form.Invoke(() => {
                    var loadFileDialog = new OpenFileDialog();
                    loadFileDialog.Title = title;
                    loadFileDialog.Filter = filter;
                    if (loadFileDialog.ShowDialog() == DialogResult.OK) {
                        selectedFileCallback(new FileInfo(loadFileDialog.FileName));
                    } else {
                        selectedFileCallback(null);
                    }
                });
            }
        }
    }
}

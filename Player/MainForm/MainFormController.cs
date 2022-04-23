using Player._Infrastructure.Controls;
using Player._Infrastructure.Events;
using Player.Events;
using System.Text;

namespace Player.MainForm {
    partial class KPC8Player {
        internal class Controller : IEventListener<LoadedProgramChangedEvent> {
            private const string MainTitle = "KPC8 Player";
            private readonly KPC8Player form;
            private string loadedFileName;
            private string statusTitle;

            public Controller(KPC8Player form) {
                this.form = form;
                this.ListenToEvent<LoadedProgramChangedEvent>();
            }

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

            internal void FreezeFrom() {
                mnuToolBar.OnUI(x => x.Enabled = false);
            }

            internal void UnfreezeForm() {
                mnuToolBar.OnUI(x => x.Enabled = true);
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
            internal ToolStripMenuItem mnuFileLoadSourceBtn => form.mnuFileLoadSourceBtn;
            internal ToolStripButton mnuPlayBtn => form.mnuPlayBtn;
            internal ToolStripButton mnuDbgBtn => form.mnuDbgBtn;
            internal ToolStripButton mnuStopBtn => form.mnuStopBtn;
            internal ToolStripButton mnuPauseBtn => form.mnuPauseBtn;
            //internal FlowLayoutPanel regsPnl => form.regsPnl;


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

            public void OnEvent(LoadedProgramChangedEvent @event) {
                if (@event.RomFile != null) {
                    LoadedFileName = @event.RomFile.Name;
                } else if (@event.SourceFile != null) {
                    LoadedFileName = @event.SourceFile.Name;
                } else {
                    LoadedFileName = null;
                }
            }

            public void SetRenderCanvasBitmap(Bitmap bitmap) {
                form.renderCanvas.Image?.Dispose();
                form.renderCanvas.Image = bitmap;
            }

            public void ResetRenderCanvas() {
                form.renderCanvas.Image?.Dispose();
                var bm = new Bitmap(1, 1);
                bm.SetPixel(0, 0, Color.Black);
                form.renderCanvas.Image = bm;
            }
        }
    }
}

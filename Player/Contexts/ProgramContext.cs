using Player.InternalForms.Dialogs;
using Player.MainForm;

namespace Player.Contexts {
    internal class ProgramContext {
        private readonly KPC8Player.Controller controller;
        private FileInfo romFile;
        private FileInfo sourceFile;

        public ProgramContext(KPC8Player.Controller controller) {
            this.controller = controller;
        }

        private const string SelectKpcSrcDialogTitle = "Select KPC8 source file";
        private const string SelectKpcSrcDialogFilter = "KPC8 source file|*.kpc|All files|*.*";

        private const string SelectKpcRomDialogTitle = "Select KPC8 ROM file";
        private const string SelectKpcRomDialogFilter = "KPC8 ROM file|*.kpcrom|All files|*.*";

        private const string SelectKpcRomOrSourceDialogTitle = "Select KPC8 ROM file or KPC8 source file";
        private const string SelectKpcRomOrSourceDialogFilter = "KPC8 ROM file|*.kpcrom|KPC8 source file|*.kpc";

        public FileInfo RomFile {
            get => romFile;
            private set {
                romFile = value;
                SetFormLoadedFileTitle();
            }
        }

        public FileInfo SourceFile {
            get => sourceFile;
            private set {
                sourceFile = value;
                SetFormLoadedFileTitle();
            }
        }

        private void SetFormLoadedFileTitle() {
            if (romFile != null) {
                controller.LoadedFileName = romFile.Name;
            } else if (sourceFile != null) {
                controller.LoadedFileName = sourceFile.Name;
            } else {
                controller.LoadedFileName = null;
            }
        }

        public bool IsRomFileSelected => RomFile != null;
        public bool IsSourceFileSelected => SourceFile != null;

        public bool TryLoadSourceFile() {
            if (LoadFileDialog.TryLoadFile(SelectKpcSrcDialogTitle, SelectKpcSrcDialogFilter, out var selectedFile)) {
                RomFile = null;
                SourceFile = selectedFile;
                return true;
            }
            return false;
        }

        public bool TryLoadRomFile() {
            if (LoadFileDialog.TryLoadFile(SelectKpcRomDialogTitle, SelectKpcRomDialogFilter, out var selectedFile)) {
                RomFile = null;
                SourceFile = selectedFile;
                return true;
            }
            return false;
        }

        public bool TryLoadSourceOrRomFile() {
            if (LoadFileDialog.TryLoadFile(SelectKpcRomOrSourceDialogTitle, SelectKpcRomOrSourceDialogFilter, out var selectedFile)) {
                if (selectedFile.Extension.Equals(".kpcrom", StringComparison.OrdinalIgnoreCase)) {
                    RomFile = selectedFile;
                } else if (selectedFile.Extension.Equals(".kpc", StringComparison.OrdinalIgnoreCase)) {
                    SourceFile = selectedFile;
                } else {
                    return false;
                }
            }

            return true;
        }
    }
}

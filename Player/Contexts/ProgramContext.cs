using Player._Infrastructure.Events;
using Player.Events;
using Player.InternalForms.Dialogs;

namespace Player.Contexts {
    internal class ProgramContext {
        private FileInfo romFile;
        private FileInfo sourceFile;

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
                FireLoadedProgramChangedEvent();
            }
        }

        public FileInfo SourceFile {
            get => sourceFile;
            private set {
                sourceFile = value;
                FireLoadedProgramChangedEvent();
            }
        }

        private void FireLoadedProgramChangedEvent() {
            KEvent.Fire(new LoadedProgramChangedEvent { RomFile = romFile, SourceFile = sourceFile });
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
                RomFile = selectedFile;
                SourceFile = null;
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

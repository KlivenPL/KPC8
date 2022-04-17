namespace Player.InternalForms.Dialogs {
    internal static class LoadFileDialog {
        public static bool TryLoadFile(string title, string filter, out FileInfo selectedFile) {
            var loadFileDialog = new OpenFileDialog();
            loadFileDialog.Title = title;
            loadFileDialog.Filter = filter;
            if (loadFileDialog.ShowDialog() == DialogResult.OK) {
                selectedFile = new FileInfo(loadFileDialog.FileName);
                return true;
            } else {
                selectedFile = null;
                return false;
            }
        }
    }
}

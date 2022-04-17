namespace Player._Infrastructure.Controls {
    internal static class OnUIThreadExtension {
        public static void OnUiThread(this ToolStripButton button, Action<ToolStripButton> action) {
            button.GetCurrentParent().Invoke(() => action(button));
        }

        public static void OnUiThread(this ToolStrip toolStrip, Action<ToolStrip> action) {
            toolStrip.Invoke(() => action(toolStrip));
        }
    }
}

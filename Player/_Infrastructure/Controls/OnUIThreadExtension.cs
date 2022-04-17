using Player.MainForm;

namespace Player._Infrastructure.Controls {
    internal static class OnUIThreadExtension {
        public static void OnUI(this ToolStripButton button, Action<ToolStripButton> action) {
            KPC8Player.InvokeOnForm(() => action(button));
        }

        public static void OnUI(this ToolStrip toolStrip, Action<ToolStrip> action) {
            KPC8Player.InvokeOnForm(() => action(toolStrip));
        }

        public static void OnUI(this ToolStripMenuItem toolStripMenuItem, Action<ToolStripMenuItem> action) {
            KPC8Player.InvokeOnForm(() => action(toolStripMenuItem));
        }
    }
}

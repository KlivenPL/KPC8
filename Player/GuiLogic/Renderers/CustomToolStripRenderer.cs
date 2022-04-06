namespace Player.Gui.Renderers {
    internal class CustomToolStripRenderer : ToolStripProfessionalRenderer {
        public CustomToolStripRenderer() : base() {

        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
            //if (!(e.ToolStrip is ToolStrip))
            //base.OnRenderToolStripBorder(e);
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e) {
            //  base.OnRenderButtonBackground(e);
        }
    }
}

using System.Drawing.Drawing2D;

namespace Player.Controls.RenderCanvas {
    internal class RenderCanvas : PictureBox {
        protected override void OnPaint(PaintEventArgs paintEventArgs) {
            paintEventArgs.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            paintEventArgs.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            base.OnPaint(paintEventArgs);
        }
    }
}

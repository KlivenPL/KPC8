using System.Drawing.Drawing2D;

namespace Player.Controls.RenderCanvas {
    internal class RenderCanvas : PictureBox {

        public void OnFormResize(int w, int h) {
            var newHeight = h;
            var newWidth = 320f / 192f * newHeight;

            Size = new Size((int)newWidth, (int)newHeight);
            Left = (w - Width) / 2;
            Top = (h - Height) / 2;
        }

        public RenderCanvas() {
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SizeMode = PictureBoxSizeMode.StretchImage;
        }

        protected override void OnPaint(PaintEventArgs paintEventArgs) {
            paintEventArgs.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            paintEventArgs.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            base.OnPaint(paintEventArgs);
        }
    }
}

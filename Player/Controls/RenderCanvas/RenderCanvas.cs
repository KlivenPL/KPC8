using System.Drawing.Drawing2D;

namespace Player.Controls.RenderCanvas {
    internal class RenderCanvas : PictureBox {
        private readonly object renderCanvasLock;
        private float aspectRatio = 1;
        private int parentWidth, parentHeight;

        public RenderCanvas(int parentWidth, int parentHeight, object renderCanvasLock) {
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SizeMode = PictureBoxSizeMode.StretchImage;

            this.parentWidth = parentWidth;
            this.parentHeight = parentHeight;
            this.renderCanvasLock = renderCanvasLock;
        }

        public void OnFormResize(int w, int h) {
            parentWidth = w; parentHeight = h;

            // lock (renderCanvasLock) {
            int newWidth, newHeight;

            if ((float)w / h > aspectRatio) {
                newWidth = (int)(h * aspectRatio);
                newHeight = h;
            } else {
                newWidth = w;
                newHeight = (int)(w / aspectRatio);
            }

            Size = new Size((int)newWidth, (int)newHeight);
            Left = (w - Width) / 2;
            Top = (h - Height) / 2;
            //}
        }

        protected override void OnPaint(PaintEventArgs paintEventArgs) {
            var aspectRatio = ((float)(Image?.Width ?? 1f)) / ((float)(Image?.Height ?? 1f));
            if (aspectRatio != this.aspectRatio) {
                this.aspectRatio = aspectRatio;
                OnFormResize(parentWidth, parentHeight);
            }

            paintEventArgs.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            paintEventArgs.Graphics.PixelOffsetMode = PixelOffsetMode.Half;

            //lock (renderCanvasLock) {
            base.OnPaint(paintEventArgs);
            // }
        }
    }
}

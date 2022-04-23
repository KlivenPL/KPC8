using System.Drawing;

namespace Runner._Infrastructure {
    internal interface IKPC8Renderer {
        bool TryRender(out Bitmap bitmap);
    }
}

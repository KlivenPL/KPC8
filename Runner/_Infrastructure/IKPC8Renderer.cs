using System.Threading;

namespace Runner._Infrastructure {
    public interface IKPC8Renderer {
        //bool TryRender(/*out Bitmap bitmap*/);
        //void RenderQuad(int displayWidth, int displayHeight);
        void BackgroundRenderLoop(CancellationToken cancellationToken);
        void ClearFrame(int displayWidth, int displayHeight);
        bool IsTextureReinitRequired();
        void RenderQuad(int displayWidth, int displayHeight);
        void SetupTexture();
        void UpdateTextureFromFrontBuffer();
    }
}

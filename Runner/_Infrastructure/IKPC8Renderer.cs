namespace Runner._Infrastructure {
    public interface IKPC8Renderer {
        void BackgroundRenderLoop();
        void ClearFrame(int displayWidth, int displayHeight);
        bool IsTextureReinitRequired();
        void RenderQuad(int displayWidth, int displayHeight);
        void SetupTexture();
        void UpdateTextureFromFrontBuffer();
    }
}

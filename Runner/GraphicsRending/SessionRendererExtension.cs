using Runner.Debugger;

namespace Runner.GraphicsRending {
    public static class SessionRendererExtension {

        public static RendererController AttachRenderer(this DebugSessionController sessionController) {
            return new RendererController(sessionController);
        }
    }
}

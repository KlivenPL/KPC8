using Runner.Debugger;
using Runner.Player;

namespace Runner.GraphicsRending {
    public static class SessionRendererExtension {

        public static RendererController AttachRenderer(this DebugSessionController sessionController) {
            return new RendererController(sessionController);
        }

        public static RendererController AttachRenderer(this PlaySessionController sessionController) {
            return new RendererController(sessionController);
        }
    }
}

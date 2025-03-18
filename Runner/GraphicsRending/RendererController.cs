using Runner._Infrastructure;
using System;
using System.Threading;

namespace Runner.GraphicsRending {
    public class RendererController {
        private readonly IKPC8Renderer renderer;
        private readonly CancellationTokenSource cts;
        private readonly IKPC8SessionController sessionController;

        private Thread renderingThread;

        internal RendererController(IKPC8SessionController sessionController) {
            // renderer = new NesLikeRenderer(sessionController.GetKPC8Build);
            renderer = new Kpc8Renderer(sessionController.GetKPC8Build);
            this.cts = new CancellationTokenSource();
            this.sessionController = sessionController;
        }

        public void StartRendering(Action<IKPC8Renderer> startRendering) {
            if (renderingThread != null) {
                throw new System.Exception("Rendering already started");
            }

            sessionController.TerminatedEvent += StopRendering;
            //sessionController.dis += OnSessionEnd;
            //this.targetFramerate = targetFramerate;
            renderingThread = new Thread(() => RenderLoop(cts.Token)) {
                Name = "KPC8 Render thread"
            };
            startRendering(renderer);
            renderingThread.IsBackground = true;
            renderingThread.Start();
        }

        private void RenderLoop(CancellationToken cancellationToken) {
            while (!cancellationToken.IsCancellationRequested) {
                renderer.BackgroundRenderLoop();
                //Thread.Sleep(16);
            }

            renderingThread = null;
        }

        private void StopRendering() {
            cts.Cancel();
        }
    }
}

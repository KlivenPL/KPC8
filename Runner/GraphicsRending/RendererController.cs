using Runner._Infrastructure;
using System;
using System.Threading;

namespace Runner.GraphicsRending {
    public class RendererController {
        private readonly IKPC8Renderer renderer;
        private readonly ManualResetEventSlim renderEvent;
        private readonly CancellationTokenSource cts;
        private readonly IKPC8SessionController sessionController;

        private Thread renderingThread;

        internal RendererController(IKPC8SessionController sessionController) {
            renderEvent = new ManualResetEventSlim(true);
            // renderer = new NesLikeRenderer(sessionController.GetKPC8Build);
            renderer = new Kpc8Renderer(sessionController.GetKPC8Build);
            this.cts = new CancellationTokenSource();
            this.sessionController = sessionController;
        }

        public void StartRendering(Action<IKPC8Renderer> startRendering) {
            if (renderingThread != null) {
                throw new System.Exception("Rendering already started");
            }

            sessionController.TerminatedEvent += OnSessionEnd;
            //sessionController.dis += OnSessionEnd;
            //this.targetFramerate = targetFramerate;
            renderingThread = new Thread(() => RenderLoop(cts.Token)) {
                Name = "KPC8 Render thread"
            };
            startRendering(renderer);
            renderingThread.IsBackground = true;
            renderingThread.Start();
        }

        public void RequestPauseRendering() {
            renderEvent.Reset();
        }

        public void RequestResumeRendering() {
            renderEvent.Set();
        }

        public void StopRendering() {
            cts.Cancel();
            RequestResumeRendering();
        }

        private void OnSessionEnd() {
            StopRendering();
        }

        private void RenderLoop(CancellationToken cancellationToken) {
            while (!cancellationToken.IsCancellationRequested) {
                renderer.BackgroundRenderLoop(cancellationToken);
                Thread.Sleep(16);
            }

            renderingThread = null;
        }
    }
}

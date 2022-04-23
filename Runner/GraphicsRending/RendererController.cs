using Runner._Infrastructure;
using System;
using System.Drawing;
using System.Threading;

namespace Runner.GraphicsRending {
    public class RendererController {
        private readonly IKPC8Renderer renderer;
        private readonly ManualResetEventSlim renderEvent;
        private readonly CancellationTokenSource cts;
        private readonly IKPC8SessionController sessionController;

        private Thread renderingThread;
        private int targetFramerate;

        public event Action<Bitmap> CanvasWriteEvent;

        internal RendererController(IKPC8SessionController sessionController) {
            renderEvent = new ManualResetEventSlim(true);
            renderer = new NesLikeRenderer(sessionController.GetKPC8Build);
            this.cts = new CancellationTokenSource();
            this.sessionController = sessionController;
        }

        public void StartRendering(int targetFramerate) {
            if (renderingThread != null) {
                throw new System.Exception("Rendering already started");
            }

            sessionController.TerminatedEvent += OnSessionEnd;
            //sessionController.dis += OnSessionEnd;
            this.targetFramerate = targetFramerate;
            renderingThread = new Thread(() => RenderLoop(cts.Token)) {
                Name = "KPC8 Render thread"
            };
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
            while (!cts.IsCancellationRequested) {
                try {
                    renderEvent.Wait(cancellationToken);
                } catch (OperationCanceledException) {
                    return;
                }

                if (renderer.TryRender(out var bitmap)) {
                    CanvasWriteEvent?.Invoke(bitmap);
                }

                Thread.Sleep(1000 / targetFramerate);
            }
        }
    }
}

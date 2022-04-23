using DebugAdapter;
using DebugAdapter.Configuration;
using Player._Configuration;
using Player._Infrastructure.Events;
using Player.Events;
using Runner.Configuration;
using Runner.Debugger;
using Runner.GraphicsRending;
using System.Net;
using System.Net.Sockets;

namespace Player.Debugger {
    internal class DapAdapterAttachInitializer {
        private TcpListener listener;
        private DebugSessionController debugSessionController;
        private CancellationTokenSource cts = new CancellationTokenSource();

        public DapAdapterAttachInitializer() {
            Application.ApplicationExit += (x, d) => Stop();
        }

        public void Stop() {
            cts.Cancel();
            if (debugSessionController == null) {
                KEvent.Fire(new DapAdapterStatusChangedEvent { Status = DapAdapterStatus.None });
            } else {
                debugSessionController.Terminate();
            }
        }

        public RendererController AttachRenderer() {
            return debugSessionController.AttachRenderer();
        }

        public void InitializeAttachServer(DebugAttachParameters attachArgs) {
            var listenThread = new Thread(async () => await ListenToConnectionThreadProc(attachArgs, cts.Token)) {
                Name = "DebugServer listener thread"
            };

            listenThread.Start();
            listenThread.Join();
        }

        private async Task ListenToConnectionThreadProc(DebugAttachParameters attachArgs, CancellationToken cancellationToken) {
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), attachArgs.ServerPort);
            listener.Start();

            KEvent.Fire(new DapAdapterStatusChangedEvent { Status = DapAdapterStatus.AwaitingConnection });

            Socket clientSocket = null;
            try {

                clientSocket = await listener.AcceptSocketAsync(cancellationToken);
            } catch (OperationCanceledException) {
                listener.Stop();
                return;
            }

            var clientThread = new Thread(() => DebugServerConnectionThreadProc(attachArgs, clientSocket)) {
                Name = "DebugServer connection thread"
            };

            clientThread.Start();
        }

        private void DebugServerConnectionThreadProc(DebugAttachParameters attachArgs, Socket clientSocket) {
            Console.WriteLine("Accepted connection");


            using (Stream stream = new NetworkStream(clientSocket)) {
                var debugSessionConfiguration = new DebugSessionConfiguration {
                    DebugSymbols = attachArgs.DebugSymbols,
                };

                var dapAdapterConfiguration = new DapAdapterConfiguration {
                    SourceFilePath = attachArgs.SourceFilePath,
                };

                var kpcConfiguration = new KPC8Configuration {
                    ClockMode = Components.Clocks.ClockMode.Manual,
                    ClockPeriodInTicks = attachArgs.KPC8ConfigurationDto.ClockPeriodInTicks ?? 3,
                    ExternalModules = attachArgs.KPC8ConfigurationDto.ExternalModules,
                    InitialRamData = attachArgs.KPC8ConfigurationDto.InitialRamData,
                    RomData = attachArgs.CompiledProgram
                };

                debugSessionController = DebugSessionController.Factory.Create(kpcConfiguration, debugSessionConfiguration);

                var dapAdapter = new DapAdapter(dapAdapterConfiguration, debugSessionController, stream, stream);

                dapAdapter.Protocol.LogMessage += (sender, e) => dapAdapter.Protocol.SendEvent(new Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Messages.OutputEvent {
                    Output = $"{e.Message}{Environment.NewLine}",
                    Category = Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Messages.OutputEvent.CategoryValue.Stderr
                });

                KEvent.Fire(new DapAdapterStatusChangedEvent { Status = DapAdapterStatus.Connected });
                dapAdapter.Run();
                dapAdapter.Protocol.WaitForReader();
            }

            Console.WriteLine("Connection closed");
            listener.Stop();

            KEvent.Fire(new DapAdapterStatusChangedEvent { Status = DapAdapterStatus.None });
        }
    }
}

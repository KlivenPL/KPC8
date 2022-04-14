using Assembler;
using Assembler.DebugData;
using DebugAdapter;
using DebugAdapter.Configuration;
using Player._Configuration.CmdLine;
using Runner.Configuration;
using Runner.Debugger;
using System.Collections;
using System.Net;
using System.Net.Sockets;

namespace Player.Debugger {
    internal class DapAdapterAttachInitializer {

        public void InitializeAttachServer(DebugArgs debugArgs) {
            BitArray[] program;
            IEnumerable<IDebugSymbol> debugSymbols;
            KPC8Configuration kpc8Configuration;

            if (!ValidateArgs(debugArgs, out var validationErrors)) {
                RunAndExitWithErrorMessage(1, string.Join(Environment.NewLine, validationErrors));
                return;
            } else if (!TryCompile(debugArgs.SourceFilePath, out program, out debugSymbols, out var compileErrorMessage)) {
                RunAndExitWithErrorMessage(2, compileErrorMessage);
                return;
            } else if (!TryLoadKPC8Configuration(debugArgs.KPC8ConfigFilePath, program, out kpc8Configuration, out var kpcConfigErrorMessage)) {
                RunAndExitWithErrorMessage(3, $"Invalid KPC8Configuration file:{Environment.NewLine}{kpcConfigErrorMessage}");
                return;
            }

            Console.WriteLine($"Waiting for connections on port {2137}...");
            Thread listenThread = new Thread(() => {
                TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 2137);
                listener.Start();

                while (true) {
                    Socket clientSocket = listener.AcceptSocket();
                    Thread clientThread = new Thread(() => {
                        Console.WriteLine("Accepted connection");

                        using (Stream stream = new NetworkStream(clientSocket)) {
                            var debugSessionConfiguration = new DebugSessionConfiguration {
                                DebugSymbols = debugSymbols,
                                StopAtEntry = debugArgs.PauseAtEntry,
                            };

                            var dapAdapterConfiguration = new DapAdapterConfiguration {
                                SourceFilePath = debugArgs.SourceFilePath,
                            };

                            var debugSessionController = DebugSessionController.Factory.Create(kpc8Configuration, debugSessionConfiguration);

                            var dapAdapter = new DapAdapter(dapAdapterConfiguration, debugSessionController, stream, stream);

                            dapAdapter.Protocol.LogMessage += (sender, e) => dapAdapter.Protocol.SendEvent(new Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Messages.OutputEvent {
                                Output = $"{e.Message}{Environment.NewLine}",
                                Category = Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Messages.OutputEvent.CategoryValue.Stderr
                            });

                            dapAdapter.Run();
                            dapAdapter.Protocol.WaitForReader();
                        }

                        Console.WriteLine("Connection closed");
                    });

                    clientThread.Name = "DebugServer connection thread";
                    clientThread.Start();
                }
            });

            listenThread.Name = "DebugServer listener thread";
            listenThread.Start();
            listenThread.Join();
        }

        private void RunAndExitWithErrorMessage(int code, string errorMessage) {
            Console.Error.WriteLine(errorMessage);

            new DapAdapter(null, null, Console.OpenStandardInput(), Console.OpenStandardOutput())
                .RunAndExitWithErrorMessage(errorMessage);

            Environment.Exit(code);
        }

        private bool ValidateArgs(DebugArgs debugArgs, out List<string> validationErrors) {
            validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(debugArgs.SourceFilePath)) {
                validationErrors.Add($"Source file path not given");
                return false;
            }

            if (!File.Exists(debugArgs.SourceFilePath)) {
                validationErrors.Add($"Could not find the source path: \"{debugArgs.SourceFilePath}\"");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(debugArgs.KPC8ConfigFilePath) && !File.Exists(debugArgs.KPC8ConfigFilePath)) {
                validationErrors.Add($"Could not find the KPC8 configuration path: \"{debugArgs.KPC8ConfigFilePath}\"");
                return false;
            }

            return true;
        }

        private bool TryLoadKPC8Configuration(string configurationFilePath, BitArray[] program, out KPC8Configuration kpc8Configuration, out string kpcConfigErrorMessage) {
            // TODO
            kpc8Configuration = new KPC8Configuration {
                ClockMode = Components.Clocks.ClockMode.Manual,
                ClockPeriodInTicks = 3,
                RomData = program,
            };
            kpcConfigErrorMessage = null;
            return true;
        }

        private bool TryCompile(string sourceFilePath, out BitArray[] program, out IEnumerable<IDebugSymbol> debugSymbols, out string compileErrorMessage) {
            debugSymbols = null;
            compileErrorMessage = null;
            program = null;

            try {
                program = Compiler.CompileFromFile(sourceFilePath, out debugSymbols);
                return true;
            } catch (Exception ex) {
                compileErrorMessage = ex.Message;
            }

            return false;
        }
    }
}

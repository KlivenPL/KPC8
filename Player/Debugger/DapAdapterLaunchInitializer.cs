using Assembler.DebugData;
using DebugAdapter;
using DebugAdapter.Configuration;
using Player._Configuration.CmdLine;
using Runner.Configuration;
using Runner.Debugger;
using System.Collections;

namespace Player.Debugger {
    internal class DapAdapterLaunchInitializer : DapAdapterInitializerBase {
        public void InitializeLaunch(DebugLaunchArgs debugArgs) {
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

            var debugSessionConfiguration = new DebugSessionConfiguration {
                DebugSymbols = debugSymbols,
            };

            var dapAdapterConfiguration = new DapAdapterConfiguration {
                SourceFilePath = debugArgs.SourceFilePath,
            };

            var debugSessionController = DebugSessionController.Factory.Create(kpc8Configuration, debugSessionConfiguration);

            var dapAdapter = new DapAdapter(dapAdapterConfiguration, debugSessionController, Console.OpenStandardInput(), Console.OpenStandardOutput());
            dapAdapter.Run();
        }

        private bool ValidateArgs(DebugLaunchArgs debugArgs, out List<string> validationErrors) {
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

        private void RunAndExitWithErrorMessage(int code, string errorMessage) {
            Console.Error.WriteLine(errorMessage);

            new DapAdapter(null, null, Console.OpenStandardInput(), Console.OpenStandardOutput())
                .RunAndExitWithErrorMessage(errorMessage);

            Environment.Exit(code);
        }
    }
}

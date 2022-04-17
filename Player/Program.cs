using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Player._Configuration;
using Player._Configuration.CmdLine;
using Player.Debugger;

namespace Player {
    internal static class Program {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            Parser.Default.ParseArguments<DefaultArgs, DebugLaunchArgs>(args)
                  .WithParsed<DefaultArgs>(defaultArgs => InstallServices(RunMainForm))
                  .WithParsed<DebugLaunchArgs>(RunNoGuiDebug);
        }

        private static void InstallServices(Action<ServiceCollection> then) {
            var services = new ServiceCollection();
            services.InstallAll();
            then(services);
        }

        private static void RunMainForm(ServiceCollection services) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.SystemAware);

            using ServiceProvider serviceProvider = services.BuildServiceProvider();
            var player = serviceProvider.GetRequiredService<MainForm.KPC8Player>();

            try {
                Application.Run(player);
            } catch (Exception) {
                throw;
            }
        }

        private static void RunNoGuiDebug(DebugLaunchArgs args) {
            new DapAdapterLaunchInitializer().InitializeLaunch(args);
        }
    }
}
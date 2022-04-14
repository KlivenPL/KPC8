using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Player._Configuration;
using Player._Configuration.CmdLine;
using Player.Debugger;
using System.Diagnostics;

namespace Player {
    internal static class Program {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            Console.WriteLine("args:" + String.Join(" ", args));
            Debug.WriteLine("args" + String.Join(" ", args));
            Parser.Default.ParseArguments<DefaultArgs, DebugArgs>(args)
                  .WithParsed<DefaultArgs>(defaultArgs => InstallServices(RunMainForm))
                  .WithParsed<DebugArgs>(RunNoGuiDebug);
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
            Application.Run(player);
        }

        private static void RunNoGuiDebug(DebugArgs args) {
            // new DapAdapterLaunchInitializer().InitializeLaunch(args);
            new DapAdapterLaunchInitializer().InitializeLaunchViaServer(args);
        }
    }
}
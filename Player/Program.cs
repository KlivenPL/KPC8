using Microsoft.Extensions.DependencyInjection;
using Player._Configuration;

namespace Player {
    internal static class Program {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.SystemAware);

            var services = new ServiceCollection();
            services.InstallAll();

            using ServiceProvider serviceProvider = services.BuildServiceProvider();
            var player = serviceProvider.GetRequiredService<MainForm.KPC8Player>();
            Application.Run(player);
        }
    }
}
using Microsoft.Extensions.DependencyInjection;
using Player.Contexts;
using Player.InternalForms.BitArrayViewer;
using Player.Loaders;

namespace Player._Configuration {
    internal static class DiInstaller {

        public static void InstallAll(this IServiceCollection services) {
            InstallForms(services);
            InstallContexts(services);
            InstallLoaders(services);
            DiStateInstaller.Install(services);
        }

        private static void InstallForms(IServiceCollection services) {
            services
                .AddScoped<MainForm.KPC8Player>()
                .AddScoped<MainForm.KPC8Player.Controller>()
                .AddTransient<BitArrayViewerForm>();
        }

        private static void InstallContexts(IServiceCollection services) {
            services
                .AddSingleton<ProgramContext>();
        }

        private static void InstallLoaders(IServiceCollection services) {
            services
                .AddSingleton<ProgramLoader>()
                .AddSingleton<KPC8ConfigurationLoader>();
        }
    }
}

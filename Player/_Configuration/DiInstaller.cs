using Microsoft.Extensions.DependencyInjection;
using Player.BitArrayViewer;

namespace Player._Configuration {
    internal static class DiInstaller {

        public static void InstallAll(this IServiceCollection services) {
            InstallForms(services);
            InstallServices(services);
        }

        private static void InstallForms(IServiceCollection services) {
            services
                .AddScoped<KPC8Player>()
                .AddTransient<BitArrayViewerForm>();
        }

        private static void InstallServices(IServiceCollection services) {

        }
    }
}

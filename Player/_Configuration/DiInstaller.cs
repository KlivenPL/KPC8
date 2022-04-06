using Microsoft.Extensions.DependencyInjection;
using Player.GuiLogic.StateMachine;
using Player.InternalForms.BitArrayViewer;

namespace Player._Configuration {
    internal static class DiInstaller {

        public static void InstallAll(this IServiceCollection services) {
            InstallServices(services);
            InstallForms(services);
            DiStateInstaller.Install(services);
        }

        private static void InstallForms(IServiceCollection services) {
            services
                .AddScoped<MainForm.KPC8Player>()
                .AddTransient<BitArrayViewerForm>();
        }

        private static void InstallServices(IServiceCollection services) {
            services
                .AddScoped<MainForm.KPC8Player.Controller>()
                .AddScoped<GuiStateManager>();
        }
    }
}

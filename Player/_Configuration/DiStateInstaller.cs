using Microsoft.Extensions.DependencyInjection;
using Player.GuiLogic.StateMachine;
using Player.GuiLogic.StateMachine.States;

namespace Player._Configuration {
    internal static class DiStateInstaller {

        public static void Install(IServiceCollection services) {
            InstallGuiStateManager(services);
            InstallGuiStates(services);
        }

        private static void InstallGuiStateManager(IServiceCollection services) {
            services.AddScoped<GuiStateManager>();
        }

        private static void InstallGuiStates(IServiceCollection services) {
            services.AddSingleton<StopGuiState>();
            services.AddSingleton<PlayGuiState>();
            services.AddSingleton<DebugGuiState>();
        }
    }
}

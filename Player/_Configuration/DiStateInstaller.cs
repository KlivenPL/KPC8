using Microsoft.Extensions.DependencyInjection;
using Player.GuiLogic.StateMachine.States;

namespace Player._Configuration {
    internal static class DiStateInstaller {

        // public delegate IGuiState GuiStateResolver<TGuiState>(GuiStateType key) where TGuiState : IGuiState;

        public static void Install(IServiceCollection services) {
            InstallGuiStates(services);
            InstallGuiStateResolver(services);
        }

        private static void InstallGuiStates(IServiceCollection services) {
            services.AddSingleton<StopGuiState>();
            services.AddSingleton<PlayGuiState>();
        }

        private static void InstallGuiStateResolver(IServiceCollection services) {
            /*services.AddSingleton<GuiStateResolver>(serviceProvider => key => {
                return serviceProvider.GetServices<IGuiState>().Single(x => x.GuiStateType == key);
            });*/
        }
    }
}

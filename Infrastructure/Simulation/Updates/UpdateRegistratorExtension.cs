using _Infrastructure.Simulation.Loops;

namespace _Infrastructure.Simulation.Updates {
    public static class UpdateRegistratorExtension {
        public static void RegisterUpdate<T>(this T update) where T : IUpdate {
            SimulationLoopBuilder.Current.AddUpdate(update);
        }

        public static void UnregisterUpdate<T>(this T update) where T : IUpdate {
            // SimulationLoop.Default.UnregisterUpdate(update);
        }
    }
}

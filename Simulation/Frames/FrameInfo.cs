namespace Simulation.Frames {
    public class FrameInfo {
        public static double DeltaTime { get; private set; }
        public static double Time { get; private set; }

        internal static void Update(double deltaTime) {
            Time += deltaTime;
            DeltaTime = deltaTime;
        }
    }
}

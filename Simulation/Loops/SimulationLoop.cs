using Simulation.Frames;
using Simulation.Updates;
using System.Collections.Generic;
using System.Diagnostics;

namespace Simulation.Loops {
    public class SimulationLoop {
        private readonly Stopwatch sw;
        private double lastUpdateDeltaTime;

        private static List<IUpdate> updates = new List<IUpdate>();
        private static List<IUpdate> newUpdates = new List<IUpdate>();
        private static List<IUpdate> deletedUpdates = new List<IUpdate>();

        public static void RegisterUpdate(IUpdate update) {
            newUpdates.Add(update);
        }

        public static void UnregisterUpdate(IUpdate update) {
            deletedUpdates.Add(update);
        }

        public SimulationLoop() {
            sw = Stopwatch.StartNew();
        }

        public void Loop() {
            if (sw.ElapsedTicks >= 100) {
                FrameInfo.Update(lastUpdateDeltaTime);
                updates.ForEach(u => u.Update());
                lastUpdateDeltaTime = (double)sw.ElapsedTicks / Stopwatch.Frequency;

                updates.RemoveAll(u => deletedUpdates.Contains(u));
                deletedUpdates.Clear();

                updates.AddRange(newUpdates);
                newUpdates.Clear();

                sw.Restart();
            }
        }
    }
}

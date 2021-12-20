using Simulation.Frames;
using Simulation.Updates;
using System.Collections.Generic;
using System.Diagnostics;

namespace Simulation.Loops {
    public class SimulationLoop {
        private readonly Stopwatch sw;
        private long lastUpdateDeltaTicks;

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
            lastUpdateDeltaTicks = sw.ElapsedTicks;
            sw.Restart();
            FrameInfo.Update(lastUpdateDeltaTicks);
            updates.ForEach(u => u.Update());

            updates.RemoveAll(u => deletedUpdates.Contains(u));
            deletedUpdates.Clear();

            updates.AddRange(newUpdates);
            newUpdates.Clear();
        }
    }
}

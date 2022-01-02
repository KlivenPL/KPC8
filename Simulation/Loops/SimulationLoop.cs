using Simulation.Frames;
using Simulation.Updates;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Simulation.Loops {
    public class SimulationLoop {
        private readonly Stopwatch sw;
        private long lastUpdateDeltaTicks;

        private static List<IUpdate> updates = new List<IUpdate>();
        private static List<IUpdate> newUpdates = new List<IUpdate>();
        private static List<IUpdate> deletedUpdates = new List<IUpdate>();

        private static bool deletionPending;
        private static bool insertionPending;

        public static void RegisterUpdate(IUpdate update) {
            newUpdates.Add(update);
            insertionPending = true;
        }

        public static void UnregisterUpdate(IUpdate update) {
            deletedUpdates.Add(update);
            deletionPending = true;
        }

        public SimulationLoop() {
            sw = Stopwatch.StartNew();
        }

        public void Loop() {
            if (deletionPending) {
                updates.RemoveAll(u => deletedUpdates.Contains(u));
                deletedUpdates.Clear();
                deletionPending = false;
            }

            if (insertionPending) {
                updates.AddRange(newUpdates);
                newUpdates.Clear();
                updates = updates.OrderByDescending(u => u.Priority).ToList();
                insertionPending = false;
            }

            lastUpdateDeltaTicks = sw.ElapsedTicks;
            sw.Restart();
            FrameInfo.Update(lastUpdateDeltaTicks);
            foreach (var update in updates) {
                update.Update();
            }
        }
    }
}

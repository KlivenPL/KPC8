using Simulation.Updates;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Simulation.Loops {
    public class SimulationLoop {
        private readonly Stopwatch sw;
        private long lastUpdateDeltaTicks;

        private static IUpdate[] updates;
        private static List<IUpdate> newUpdates = new List<IUpdate>();
        /*private static List<IUpdate> deletedUpdates = new List<IUpdate>();*/

        private static bool deletionPending;
        private static bool insertionPending;

        public static void RegisterUpdate(IUpdate update) {
            newUpdates.Add(update);
            insertionPending = true;
        }

        public static void UnregisterUpdate(IUpdate update) {
            /*deletedUpdates.Add(update);
            deletionPending = true;*/
        }

        public SimulationLoop() {
            updates = new IUpdate[0];
            //sw = Stopwatch.StartNew();
        }

        public void Loop() {
            /*if (deletionPending) {
                updates.RemoveAll(u => deletedUpdates.Contains(u));
                deletedUpdates.Clear();
                deletionPending = false;
            }*/

            if (insertionPending) {
                var tmp = updates.ToList();
                tmp.AddRange(newUpdates);
                updates = tmp.ToArray();
                newUpdates.Clear();
                updates = updates.OrderByDescending(u => u.Priority).ToArray();
                insertionPending = false;
            }

            //lastUpdateDeltaTicks = sw.ElapsedTicks;
            // sw.Restart();
            //FrameInfo.Update(lastUpdateDeltaTicks);
            for (int i = 0; i < updates.Length; i++) {
                updates[i].Update();
            }

            /*foreach (var update in updates) {
                update.Update();
            }*/
        }
    }
}

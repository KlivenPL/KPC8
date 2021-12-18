using Simulation.Frames;
using Simulation.Updates;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Simulation.Loops {
    public class SimulationLoop {
        private List<IUpdate> updates;
        private Stopwatch sw;
        private double lastUpdateDeltaTime;

        public SimulationLoop(IEnumerable<IUpdate> updates) {
            this.updates = updates.ToList();
            sw = Stopwatch.StartNew();
        }


        public void Loop() {
            if (sw.ElapsedTicks >= 100) {
                FrameInfo.Update(lastUpdateDeltaTime);
                updates.ForEach(u => u.Update());
                lastUpdateDeltaTime = (double)sw.ElapsedTicks / Stopwatch.Frequency;
                sw.Restart();
            }
        }
    }
}

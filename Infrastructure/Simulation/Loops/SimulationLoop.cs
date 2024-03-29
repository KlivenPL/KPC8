﻿using _Infrastructure.Simulation.Updates;
using System;

namespace _Infrastructure.Simulation.Loops {
    public class SimulationLoop : IDisposable {
        private readonly IUpdate[] updates;
        private readonly int length;

        internal SimulationLoop(string loopName, IUpdate[] updates) {
            LoopName = loopName;
            this.updates = updates;
            length = updates.Length;
        }

        public string LoopName { get; }

        public void Loop() {
            for (int i = 0; i < length; i++) {
                updates[i].Update();
            }
        }

        public void Dispose() {
            foreach (var update in updates) {
                update.Dispose();
            }
        }
    }
}

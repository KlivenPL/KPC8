using Simulation.Updates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Simulation.Loops {
    public class SimulationLoopBuilder : IDisposable {
        [ThreadStatic] private static Stack<SimulationLoopBuilder> buildersStack;
        private readonly List<IUpdate> newUpdates;
        private readonly string loopName;
        //private static object lockObj;

        public SimulationLoopBuilder(string loopName) {
            //lockObj = new object();
            this.loopName = loopName;
            newUpdates = new List<IUpdate>();
            //  lock (lockObj) {
            buildersStack ??= new Stack<SimulationLoopBuilder>();
            buildersStack.Push(this);
            //  }
        }

        public static SimulationLoopBuilder Current {
            get {
                // lock (lockObj) {
                return buildersStack.Peek();
                // }
            }
        }

        public SimulationLoopBuilder AddUpdate(IUpdate update) {
            newUpdates.Add(update);
            return this;
        }

        public SimulationLoop Build() {
            return new SimulationLoop(loopName, newUpdates.OrderByDescending(u => u.Priority).ToArray());
        }

        public void Dispose() {
            //lock (lockObj) {
            if (buildersStack.Peek() == this) {
                buildersStack.Pop();
            } else {
                throw new Exception("Invalid order of simulation loop builders");
            }
            // }
        }
    }
}

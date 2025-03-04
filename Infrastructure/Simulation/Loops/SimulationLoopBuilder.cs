using _Infrastructure.Simulation.Updates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _Infrastructure.Simulation.Loops {
    public class SimulationLoopBuilder : IDisposable {
        [ThreadStatic] private static Stack<SimulationLoopBuilder> buildersStack;
        private readonly List<IUpdate> newUpdates;
        private string loopName;

        private SimulationLoopBuilder() {
            newUpdates = new List<IUpdate>();
        }

        public static SimulationLoopBuilder Create() {
            return new SimulationLoopBuilder();
        }

        public static SimulationLoopBuilder CreateAsCurrent() {
            return new SimulationLoopBuilder().SetAsCurrent();
        }

        public static SimulationLoopBuilder Current => buildersStack.Peek();

        public SimulationLoopBuilder AddUpdate(IUpdate update) {
            newUpdates.Add(update);
            return this;
        }

        public SimulationLoopBuilder SetName(string loopName) {
            this.loopName = loopName;
            return this;
        }

        public SimulationLoopBuilder SetAsCurrent() {
            buildersStack ??= new Stack<SimulationLoopBuilder>();
            buildersStack.Push(this);
            return this;
        }

        public SimulationLoop Build() {
            return new SimulationLoop(loopName, newUpdates.OrderByDescending(u => u.Priority).ToArray());
        }

        public void Dispose() {
            if (buildersStack != null) {
                if (buildersStack.Peek() == this) {
                    buildersStack.Pop();
                } else {
                    throw new Exception("Invalid order of simulation loop builders");
                }
            }
        }
    }
}

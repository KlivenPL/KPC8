using System;

namespace Simulation.Updates {
    public interface IUpdate : IDisposable {
        void Update();
    }
}

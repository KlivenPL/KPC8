using System;

namespace Simulation.Updates {
    public interface IUpdate : IDisposable {
        int Priority => 0;
        void Update();
    }
}

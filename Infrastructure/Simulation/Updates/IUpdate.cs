using System;

namespace _Infrastructure.Simulation.Updates {
    public interface IUpdate : IDisposable {
        int Priority => 0;
        void Update();
    }
}

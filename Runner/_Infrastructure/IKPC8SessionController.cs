using Runner.Build;
using System;

namespace Runner._Infrastructure {
    public interface IKPC8SessionController {
        event Action<int> ExitedEvent;
        event Action TerminatedEvent;

        internal KPC8Build GetKPC8Build { get; }
    }
}

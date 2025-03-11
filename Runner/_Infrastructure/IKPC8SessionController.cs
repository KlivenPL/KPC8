using Abstract;
using System;

namespace Runner._Infrastructure {
    public interface IKPC8SessionController {
        event Action<int> ExitedEvent;
        event Action TerminatedEvent;

        internal IKpcBuild GetKPC8Build { get; }
    }
}

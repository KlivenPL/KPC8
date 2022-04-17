using Player._Infrastructure.Events;
using Player.Debugger;

namespace Player.Events {
    internal class DapAdapterStatusChangedEvent : IEvent {
        public DapAdapterStatus Status { get; init; }
    }
}

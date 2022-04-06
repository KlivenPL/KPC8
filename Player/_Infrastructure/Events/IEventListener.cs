namespace Player._Infrastructure.Events {
    public interface IEventListener<in TEvent>
        where TEvent : IEvent {
        void OnEvent(TEvent @event);
    }
}

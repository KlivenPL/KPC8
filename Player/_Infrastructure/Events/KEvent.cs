namespace Player._Infrastructure.Events {
    public static class KEvent {
        public static void Fire<TEvent>(TEvent @event) where TEvent : class, IEvent {
            var eventType = typeof(TEvent);
            if (KEventListener.TryGetListeners(eventType, out var tuple)) {
                foreach (var listener in tuple.listeners) {
                    tuple.methodInfo.Invoke(listener, new[] { @event });
                }
            }
        }
    }
}

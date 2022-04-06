using Player._Infrastructure.Collections;
using System.Reflection;

namespace Player._Infrastructure.Events {
    public static class KEventListener {
        private static readonly Cache<Type, object> eventListenersCache = new Cache<Type, object>();
        private static readonly Dictionary<Type, MethodInfo> eventMethodInfos = new Dictionary<Type, MethodInfo>();

        public static void ListenToEvent<TEvent>(this IEventListener<TEvent> listener) where TEvent : IEvent {
            var eventType = typeof(TEvent);
            eventListenersCache.Add(eventType, listener);

            if (!eventMethodInfos.ContainsKey(eventType)) {
                eventMethodInfos.Add(eventType, GetOnEventMethodInfo(eventType, typeof(IEventListener<TEvent>)));
            }
        }

        public static void StopListenToEvent<TEvent>(this IEventListener<TEvent> listener) where TEvent : IEvent {
            var eventType = typeof(TEvent);
            if (!eventListenersCache.TryRemove(eventType, listener)) {
                throw new Exception($"Cannot stop listen to event {eventType.Name} of listener {listener} as it had not listened to it before.");
            }
        }

        public static bool TryGetListeners(Type eventType, out (IEnumerable<object> listeners, MethodInfo methodInfo) tuple) {
            if (eventMethodInfos.TryGetValue(eventType, out var methodInfo)) {
                var listeners = eventListenersCache[eventType];
                tuple = (listeners, methodInfo);
                return true;
            }

            tuple = default;
            return false;
        }

        private static MethodInfo GetOnEventMethodInfo(Type eventType, Type listenerType) {
            return listenerType.GetMethod(nameof(IEventListener<IEvent>.OnEvent), BindingFlags.Instance | BindingFlags.Public, null, new Type[] { eventType }, null);
        }
    }
}

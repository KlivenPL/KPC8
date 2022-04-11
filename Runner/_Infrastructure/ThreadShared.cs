using System;

namespace Runner._Infrastructure {
    internal class ThreadShared<T> {

        private readonly object _lock;
        private readonly T value;

        public ThreadShared(T value) {
            _lock = new object();
            this.value = value;
        }

        public V Get<V>(Func<T, V> getFunc) where V : struct {
            V result = default;

            lock (_lock) {
                result = getFunc(value);
            }

            return result;
        }
    }
}

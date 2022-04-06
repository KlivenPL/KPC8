using Player._Infrastructure.Collections;
namespace Player._Infrastructure.Collections
{
    public class Cache<TKey, TValue> where TValue : class {
        private MultiDictionary<TKey, WeakReference<TValue>> cache = new MultiDictionary<TKey, WeakReference<TValue>>();

        public void Add(TKey key, TValue value) {
            cache.Add(key, new WeakReference<TValue>(value));
            Clean();
        }

        public bool TryRemove(TKey key, TValue value) {
            var valToRemove = cache[key].FirstOrDefault(c => {
                if (c.TryGetTarget(out var target)) {
                    return target == value;
                }
                return false;
            });

            if (valToRemove != null) {
                return cache.TryRemove(key, valToRemove);
            }

            return false;
        }

        public IEnumerable<TValue> this[TKey key] {
            get {
                var cachedReferences = cache[key];

                if (cachedReferences != null) {
                    foreach (var reference in cachedReferences) {
                        if (reference.TryGetTarget(out var target)) {
                            yield return target;
                        }
                    }
                }
            }
        }

        private void Clean() {
            foreach (var kvp in cache) {
                var entriesRemoved = kvp.Value.RemoveAll(reference => !reference.TryGetTarget(out var target));
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;

namespace _Infrastructure.Collections {
    public class MultiDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, List<TValue>>> {
        private Dictionary<TKey, List<TValue>> dict = new Dictionary<TKey, List<TValue>>();

        public void Add(TKey key, TValue value) {
            if (dict.TryGetValue(key, out var references)) {
                references.Add(value);
            } else {
                dict.Add(key, new List<TValue> { value });
            }
        }

        public bool TryRemove(TKey key, TValue value) {
            if (dict.TryGetValue(key, out var coll)) {
                return coll.Remove(value);
            }

            return false;
        }

        public bool TryRemove(TKey key) {
            if (dict.TryGetValue(key, out var coll)) {
                coll.Clear();
                return true;
            }

            return false;
        }

        public void Clear() {
            dict.Clear();
        }

        public IEnumerator<KeyValuePair<TKey, List<TValue>>> GetEnumerator() {
            return dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return dict.GetEnumerator();
        }

        public IEnumerable<TValue> this[TKey key] {
            get {
                if (dict.TryGetValue(key, out var cachedReferences)) {
                    return cachedReferences;
                }

                return null;
            }
        }
    }
}

namespace Player._Infrastructure.Collections
{
    public class Map<T1, T2> {
        private Dictionary<T1, T2> _forward = new Dictionary<T1, T2>();
        private Dictionary<T2, T1> _reverse = new Dictionary<T2, T1>();

        public Map() {
            this.Forward = new Indexer<T1, T2>(_forward);
            this.Reverse = new Indexer<T2, T1>(_reverse);
        }

        public class Indexer<T3, T4> {
            private Dictionary<T3, T4> _dictionary;
            public Indexer(Dictionary<T3, T4> dictionary) {
                _dictionary = dictionary;
            }
            public T4 this[T3 index] {
                get { return _dictionary[index]; }
                set { _dictionary[index] = value; }
            }
        }

        public void Add(T1 t1, T2 t2) {
            _forward.Add(t1, t2);
            _reverse.Add(t2, t1);
        }

        public T2 GetForwardValue(T1 key) {
            return Forward[key];
        }

        public T1 GetReverseValue(T2 key) {
            return Reverse[key];
        }

        public bool TryGetForwardValue(T1 key, out T2 value) {
            try {
                value = Forward[key];
                return true;
            } catch {
                value = default;
                return false;
            }
        }

        public bool TryGetReverseValue(T2 key, out T1 value) {
            try {
                value = Reverse[key];
                return true;
            } catch {
                value = default;
                return false;
            }
        }

        public Indexer<T1, T2> Forward { get; private set; }
        public Indexer<T2, T1> Reverse { get; private set; }
    }

    public static class DictionaryExtentions {
        public static Map<T1, T2> ToMap<T1, T2>(this Dictionary<T1, T2> dict) {
            Map<T1, T2> map = new Map<T1, T2>();
            foreach (var keyValuePair in dict) {
                map.Add(keyValuePair.Key, keyValuePair.Value);
            }
            return map;
        }
    }
}

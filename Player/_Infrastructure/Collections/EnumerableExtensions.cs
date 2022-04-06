namespace Player._Infrastructure.Collections
{
    public static class EnumerableExtensions {
        private static Random Random = new Random();

        public static T PickRandom<T>(this IEnumerable<T> collection) {
            if (collection.Any()) {
                int index = Random.Next(0, collection.Count());
                return collection.ElementAt(index);
            }

            return default;
        }
    }
}

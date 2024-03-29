﻿namespace Player._Infrastructure.Collections
{
    public static class ListExtensions {
        public static void Move<T>(this List<T> list, int oldIndex, int newIndex) {
            var item = list[oldIndex];

            list.RemoveAt(oldIndex);

            if (newIndex > oldIndex) newIndex--;

            list.Insert(newIndex, item);
        }

        public static void Move<T>(this List<T> list, T item, int newIndex) {
            if (item != null) {
                var oldIndex = list.IndexOf(item);
                if (oldIndex > -1) {
                    list.RemoveAt(oldIndex);

                    if (newIndex > oldIndex) newIndex--;

                    list.Insert(newIndex, item);
                }
            }
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _Infrastructure.Paths {
    public static class PathComparer {
        public static bool Compare(string path1, string path2) {
            return string.Equals(
                Path.GetFullPath(path1).TrimEnd('\\'),
                Path.GetFullPath(path2).TrimEnd('\\'),
                System.StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool Contains(string path1, IEnumerable<string> pathCollection) {
            return pathCollection.Any(x => Compare(path1, x));
        }

        public static bool ComparePath(this string path1, string path2) {
            return string.Equals(
                Path.GetFullPath(path1).TrimEnd('\\'),
                Path.GetFullPath(path2).TrimEnd('\\'),
                System.StringComparison.InvariantCultureIgnoreCase);
        }
    }
}

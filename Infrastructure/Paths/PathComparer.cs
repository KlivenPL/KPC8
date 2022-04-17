using System.IO;

namespace _Infrastructure.Paths {
    public static class PathComparer {
        public static bool Compare(string path1, string path2) {
            return string.Equals(
                Path.GetFullPath(path1).TrimEnd('\\'),
                Path.GetFullPath(path2).TrimEnd('\\'),
                System.StringComparison.InvariantCultureIgnoreCase);
        }
    }
}

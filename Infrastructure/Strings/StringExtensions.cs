using System;

namespace _Infrastructure.Strings {
    public static class StringExtensions {
        public static string Left(this string input, int count) {
            return input.Substring(0, Math.Min(input.Length, count));
        }

        public static string Right(this string input, int count) {
            return input.Substring(Math.Max(input.Length - count, 0), Math.Min(count, input.Length));
        }
    }
}

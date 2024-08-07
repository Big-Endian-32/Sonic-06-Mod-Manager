using SonicNextModManager.Metadata;

namespace SonicNextModManager.Helpers
{
    public class StringHelper
    {
        /// <summary>
        /// Returns a new path with the specified filename.
        /// </summary>
        public static string ReplaceFilename(string in_path, string in_newFileName)
            => Path.Combine(Path.GetDirectoryName(in_path), Path.GetFileName(in_newFileName));

        /// <summary>
        /// Returns a combined URL path structure.
        /// </summary>
        /// <param name="in_paths">Path structure to combine.</param>
        public static string URLCombine(params string[] in_paths)
            => string.Join('/', in_paths);

        /// <summary>
        /// Returns a platform type from the extension of a file path.
        /// </summary>
        /// <param name="in_path">Path to file.</param>
        public static EPlatform GetPlatformFromFilePath(string? in_path)
        {
            if (string.IsNullOrEmpty(in_path))
                return EPlatform.Xbox;

            return Path.GetExtension(in_path).ToLower() switch
            {
                ".xex" => EPlatform.Xbox,
                ".bin" => EPlatform.PlayStation,
                _      => EPlatform.Xbox,
            };
        }

        /// <summary>
        /// Omits a prefix from the file name in a given path.
        /// </summary>
        /// <param name="in_path">The path containing the file name with the prefix.</param>
        /// <param name="in_prefix">The prefix to omit.</param>
        /// <returns>The full path with the file name without the prefix.</returns>
        public static string OmitFileNamePrefix(string in_path, string in_prefix)
        {
            if (string.IsNullOrEmpty(in_path))
                return in_path;

            var dir  = Path.GetDirectoryName(in_path);
            var name = Path.GetFileName(in_path)[in_prefix.Length..];

            if (string.IsNullOrEmpty(dir))
                return name;

            return Path.Combine(dir, name);
        }

        /// <summary>
        /// Omits a prefix from the file name in a given path.
        /// </summary>
        /// <param name="in_path">The path containing the file name with the prefix.</param>
        /// <param name="in_prefix">The prefix to omit.</param>
        /// <returns>The full path with the file name without the prefix.</returns>
        public static string OmitFileNamePrefix(string in_path, char in_prefix)
        {
            if (string.IsNullOrEmpty(in_path))
                return in_path;

            var dir = Path.GetDirectoryName(in_path);
            var name = Path.GetFileName(in_path).TrimStart(in_prefix);

            if (string.IsNullOrEmpty(dir))
                return name;

            return Path.Combine(dir, name);
        }

        /// <summary>
        /// Gets a list of indices where the input substring is found in a given string.
        /// </summary>
        /// <param name="in_str">The string to find substrings in.</param>
        /// <param name="in_substr">The substring to search for.</param>
        /// <returns>A list of indices where a substring appears in the given string.</returns>
        public static List<int> GetSubstringIndices(string in_str, string in_substr)
        {
            var indices = new List<int>();

            int i = 0;
            while ((i = in_str.IndexOf(in_substr, i)) != -1)
            {
                indices.Add(i);
                i += in_substr.Length;
            }

            return indices;
        }
    }
}

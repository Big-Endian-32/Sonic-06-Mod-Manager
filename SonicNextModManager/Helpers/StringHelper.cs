namespace SonicNextModManager.Helpers
{
    public class StringHelper
    {
        /// <summary>
        /// Returns a new path with the specified filename.
        /// </summary>
        public static string ReplaceFilename(string path, string newFile)
            => Path.Combine(Path.GetDirectoryName(path), Path.GetFileName(newFile));

        /// <summary>
        /// Removes the source directory from the input path.
        /// </summary>
        /// <param name="path">Path to remove source from.</param>
        /// <param name="source">Source directory to remove.</param>
        public static string OmitSourceDirectory(string path, string source)
            => path.Remove(0, source.Length + 1);

        /// <summary>
        /// Returns a combined URL path structure.
        /// </summary>
        /// <param name="paths">Path structure to combine.</param>
        public static string URLCombine(params string[] paths)
            => string.Join('/', paths);

        /// <summary>
        /// Returns a platform type from the extension of a file path.
        /// </summary>
        /// <param name="path">Path to file.</param>
        public static Platform GetPlatformFromFilePath(string path)
        {
            return Path.GetExtension(path).ToLower() switch
            {
                ".xex" => Platform.Xbox,
                ".bin" => Platform.PlayStation,
                _ => Platform.Xbox,
            };
        }
    }
}

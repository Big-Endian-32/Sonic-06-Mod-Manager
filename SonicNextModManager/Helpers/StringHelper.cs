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
        public static Platform GetPlatformFromFilePath(string in_path)
        {
            return Path.GetExtension(in_path).ToLower() switch
            {
                ".xex" => Platform.Xbox,
                ".bin" => Platform.PlayStation,
                _      => Platform.Xbox,
            };
        }
    }
}

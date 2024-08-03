namespace SonicNextModManager.Helpers
{
    public class DirectoryHelper
    {
        /// <summary>
        /// Returns whether or not a directory contains a resulting file based off a wildcard.
        /// </summary>
        /// <param name="path">Directory to search.</param>
        /// <param name="wildcard">Wildcard to use.</param>
        /// <param name="searchOption">Search option for querying.</param>
        public static bool Contains(string path, string wildcard, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            foreach (string file in Directory.EnumerateFiles(path, wildcard, searchOption))
                return true;

            return false;
        }

        /// <summary>
        /// Returns whether or not a directory contains a resulting file based off a wildcard.
        /// </summary>
        /// <param name="path">Directory to search.</param>
        /// <param name="wildcard">Wildcard to use.</param>
        /// <param name="single">First result to output.</param>
        public static bool Contains(string path, string wildcard, out string single)
        {
            foreach (string file in Directory.EnumerateFiles(path, wildcard, SearchOption.TopDirectoryOnly))
            {
                single = file;
                return true;
            }

            single = string.Empty;
            return false;
        }
    }
}

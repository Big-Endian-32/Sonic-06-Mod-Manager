using System.Threading.Tasks;

namespace SonicNextModManager.Helpers
{
    public class ArchiveHelper
    {
        /// <summary>
        /// Reads a requested archive.
        /// If it has not been previously read, then we store it for future access.
        /// </summary>
        /// <param name="archivePath">The path to the archive to read.</param>
        public static async Task<Marathon.Formats.Archive.U8Archive> ReadArchive(string archivePath)
        {
            // If our archive list doesn't contain this archive, then read it and store it.
            if (!App.Archives.ContainsKey(archivePath))
                App.Archives.Add(archivePath, new(archivePath, Marathon.IO.ReadMode.IndexOnly));

            // Return the archive from our list with this path's key.
            return App.Archives[archivePath];
        }

        /// <summary>
        /// Repacks any archives waiting in memory.
        /// </summary>
        public static async Task PackArchives()
        {
            // Loop through each archive in our list.
            foreach (var archive in App.Archives)
            {
                // If we haven't already, take a backup of the original file.
                if (!File.Exists($"{archive.Key}.06mm_backup"))
                    File.Copy($"{archive.Key}", $"{archive.Key}.06mm_backup");

                // Resave the modified archive.
                archive.Value.Save(archive.Key);
            }

            // Clear the archive list.
            App.Archives.Clear();
        }
    }
}

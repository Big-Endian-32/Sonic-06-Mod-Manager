using Marathon.Formats.Archive;
using SonicNextModManager.Metadata;

namespace SonicNextModManager.Helpers
{
    public class ArchiveHelper
    {
        /// <summary>
        /// Determines whether a given path is for an internal archive path.
        /// </summary>
        /// <param name="in_path">The path to a file inside the given archive in the path.</param>
        public static bool IsInternalArchivePath(string in_path)
        {
            return in_path.Contains(".arc/");
        }

        /// <summary>
        /// Gets the path to the archive from an absolute internal path.
        /// </summary>
        /// <param name="in_path">The path to a file inside the given archive in the path.</param>
        public static string GetArchivePath(string in_path)
        {
            if (IsInternalArchivePath(in_path))
            {
                var arcIndex = in_path.IndexOf(".arc/") + 5;
                var arcPath = in_path[..(arcIndex - 1)];

                return arcPath;
            }

            return in_path;
        }

        /// <summary>
        /// Gets the relative path from inside the given archive.
        /// </summary>
        /// <param name="in_path">The path to a file inside the given archive in the path.</param>
        public static string GetRelativeArchivePath(string in_path)
        {
            return Path.GetRelativePath(GetArchivePath(in_path), in_path);
        }

        /// <summary>
        /// Gets an archive file from an absolute internal path.
        /// </summary>
        /// <param name="in_path">The path to a file inside the given archive in the path.</param>
        public static U8ArchiveFile? GetArchiveFile(string in_path, bool in_isDecompress = true)
        {
            var arc = Database.LoadArchive(GetArchivePath(in_path));

            if (arc == null)
                return null;

            var filePath = GetRelativeArchivePath(in_path);

            if (!arc.Root.FileExists(filePath))
            {
                var file = new U8ArchiveFile() { Name = Path.GetFileName(filePath) };

                // Create dummy file at this directory for modifications.
                arc.Root.CreateDirectories(Path.GetDirectoryName(filePath)).Add(file);

                return file;
            }

            if (Marathon.Helpers.ArchiveHelper.GetFile(arc.Root, GetRelativeArchivePath(in_path)) is U8ArchiveFile out_file)
            {
                if (in_isDecompress)
                    out_file.Decompress();

                return out_file;
            }

            return null;
        }

        /// <summary>
        /// Gets an archive directory from an absolute internal path.
        /// </summary>
        /// <param name="in_path">The path to a file inside the given archive in the path.</param>
        public static U8ArchiveDirectory? GetArchiveDirectory(string in_path)
        {
            var arc = Database.LoadArchive(GetArchivePath(in_path));

            if (arc == null)
                return null;

            var relativePath = GetRelativeArchivePath(in_path);

            if (relativePath == ".")
                return arc.Root;

            return arc.Root.CreateDirectories(relativePath);
        }
    }
}

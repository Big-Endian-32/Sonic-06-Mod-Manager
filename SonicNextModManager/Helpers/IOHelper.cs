namespace SonicNextModManager.Helpers
{
    public class IOHelper
    {
        /// <summary>
        /// Backs up a file to the same location with a suffix.
        /// </summary>
        /// <param name="in_path">The path to the file to back up.</param>
        /// <param name="in_suffix">The suffix to append.</param>
        /// <param name="in_isOverwrite">Determines whether to overwrite an already existing backup.</param>
        /// <returns><c>true</c> if the backup was successful; otherwise, <c>false</c>.</returns>
        public static bool Backup(string in_path, string in_suffix = ".06mm_backup", bool in_isOverwrite = false)
        {
            var backupPath = in_path + in_suffix;

            if (!File.Exists(in_path))
                return false;

            if (!in_isOverwrite && File.Exists(backupPath))
                return false;

            File.Copy(in_path, backupPath, in_isOverwrite);

            return true;
        }
    }
}

using Marathon.Formats.Archive;
using Marathon.IO;
using SonicNextModManager.Helpers;
using SonicNextModManager.Patches;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SonicNextModManager.Metadata
{
    public class Mod : MetadataBase
    {
        /// <summary>
        /// The version string for this mod.
        /// </summary>
        public string? Version { get; set; } = "1.0.0";

        /// <summary>
        /// A collection of patches required by this mod.
        /// </summary>
        public ObservableCollection<string> Patches { get; set; } = [];

        /// <summary>
        /// A collection of DLL modules required by this mod.
        /// </summary>
        public ObservableCollection<string> Modules { get; set; } = [];

        public Mod() { }

        public Mod(string? in_file)
        {
            if (!File.Exists(in_file))
                throw new FileNotFoundException($"Metadata file does not exist: {in_file}");

            var mod = JsonConvert.DeserializeObject<Mod>(File.ReadAllText(in_file)) ??
                throw new JsonException("Failed to parse mod metadata.");

            Title       = mod.Title;
            Author      = mod.Author;
            Platform    = mod.Platform;
            Date        = mod.Date;
            Description = mod.Description;
            Location    = in_file;
            Version     = mod.Version;
            Patches     = mod.Patches;
            Modules     = mod.Modules;

            // Get single thumbnail and use that as the path.
            if (DirectoryHelper.Contains(Path.GetDirectoryName(in_file)!, "thumbnail.*", out string out_thumbnail))
                Thumbnail = out_thumbnail;
        }

        public static Mod Parse(string? in_file)
        {
            return new Mod(in_file);
        }

        public static void Write(Mod in_mod, string? in_file)
        {
            ArgumentException.ThrowIfNullOrEmpty(in_file);

            File.WriteAllText(in_file, JsonConvert.SerializeObject(in_mod, Formatting.Indented));
        }

        public void Write(string? in_file)
        {
            Write(this, in_file);
        }

        public void Write()
        {
            Write(this, Location);
        }

        public void Install()
        {
            var gameDirectory = App.Settings.GetGameDirectory();
            var modDirectory  = Path.GetDirectoryName(Location);

            if (string.IsNullOrEmpty(gameDirectory) || string.IsNullOrEmpty(modDirectory))
                return;

            State = EInstallState.Installing;

            var customArchives = new List<string>();

            // Loop through each file in this mod.
            foreach (var file in Directory.GetFiles(modDirectory, "*", SearchOption.AllDirectories))
            {
                var fileName = Path.GetFileName(file);
                var extension = Path.GetExtension(file);

                // Skip mod metadata.
                if (fileName == "mod.json" ||
                    extension == ".mlua" ||
                    Patches.Any(x => x.TrimStart(Database.AppendChar, Database.RemoveChar).Contains(fileName)) ||
                    Path.GetFileNameWithoutExtension(file) == "thumbnail")
                {
                    continue;
                }

                var relativePath = Path.GetRelativePath(modDirectory, file);
                var absolutePath = Path.Combine(gameDirectory, relativePath);
                var isArchive = extension == ".arc";

                // Check if this file is a custom file.
                if (fileName.StartsWith(Database.CustomChar))
                {
                    // Add archive to queue for appending to archive.pkg.
                    if (isArchive)
                        customArchives.Add(relativePath);

                    File.Copy(file, Path.Combine(gameDirectory, StringHelper.OmitFileNamePrefix(relativePath, Database.CustomChar)), true);

                    continue;
                }

                // Check if this file is an append archive.
                if (isArchive && fileName.StartsWith(Database.AppendChar))
                {
                    var baseArchive  = Database.LoadArchive(StringHelper.OmitFileNamePrefix(relativePath, Database.AppendChar));
                    var mergeArchive = new U8Archive(file, ReadMode.IndexOnly);

                    if (baseArchive == null)
                        throw new Exception("Failed to merge archives as the base archive returned null.");

                    // Merge the two archives together.
                    Marathon.Helpers.ArchiveHelper.MergeWith(baseArchive.Root, mergeArchive.Root);

                    continue;
                }

                // Check if this file exists in the game directory.
                if (File.Exists(absolutePath))
                {
                    Database.Backup(absolutePath);

                    File.Copy(file, absolutePath, true);

                    /* Remove the archive from the merge list, 
                       as it has been overwritten entirely. */
                    Database.Archives!.Remove(absolutePath);
                }
            }

            // Append custom archives to archive.pkg.
            using (var arcList = new ArchiveList())
                arcList.Add(customArchives);

            foreach (var patchName in Patches)
            {
                // Check if this patch should be installed.
                if (patchName.StartsWith(Database.AppendChar))
                {
                    var patchPath = Path.Combine(modDirectory, patchName.TrimStart(Database.AppendChar));

                    if (!File.Exists(patchPath))
                    {
                        /* Search for this patch in the patches directory,
                           if it doesn't exist in the mod's directory. */
                        patchPath = Path.Combine(App.Directories["Patches"], patchName);

                        if (!Directory.Exists(patchPath))
                            continue;

                        // Load patch script from patches directory.
                        patchPath = Path.Combine(patchPath, "patch.lua");
                    }

                    var patch = Patch.Parse(patchPath);

                    patch.Install();
                }

                // Check if this patch should be blocked.
                if (patchName.StartsWith(Database.RemoveChar))
                {
                    // TODO
                }
            }

            State = EInstallState.Installed;
        }

        public void Uninstall()
        {
            var gameDirectory = App.Settings.GetGameDirectory();
            var modDirectory  = Path.GetDirectoryName(Location);

            if (string.IsNullOrEmpty(gameDirectory) || string.IsNullOrEmpty(modDirectory))
                return;

            State = EInstallState.Uninstalling;

            // Remove custom files.
            foreach (var file in Directory.GetFiles(modDirectory, $"{Database.CustomChar}*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(modDirectory, file);
                var originalPath = Path.Combine(gameDirectory, StringHelper.OmitFileNamePrefix(relativePath, Database.CustomChar));

                if (!File.Exists(originalPath))
                    continue;

                File.Delete(originalPath);
            }

            State = EInstallState.Idle;
        }
    }
}

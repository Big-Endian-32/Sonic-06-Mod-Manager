using Marathon.Formats.Archive;
using Marathon.Helpers;
using Marathon.IO;
using SonicNextModManager.Helpers;
using System.Collections.ObjectModel;

namespace SonicNextModManager.Metadata
{
    public class Mod : MetadataBase
    {
        /// <summary>
        /// The version string for this mod.
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// A collection of patches required by this mod.
        /// </summary>
        public ObservableCollection<string> Patches { get; set; } = [];

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

            State = InstallState.Installing;

            // Loop through each file in this mod.
            foreach (var file in Directory.GetFiles(modDirectory, "*", SearchOption.AllDirectories))
            {
                var fileName = Path.GetFileName(file);

                // Skip mod metadata.
                if (fileName == "mod.json" ||
                    Path.GetExtension(fileName) == ".mlua" ||
                    Patches.Any(x => x.Contains(fileName)) ||
                    Path.GetFileNameWithoutExtension(file) == "thumbnail")
                {
                    continue;
                }

                var relativePath = Path.GetRelativePath(modDirectory, file);
                var absolutePath = Path.Combine(gameDirectory, relativePath);

                // Check if this file is a custom file.
                if (fileName.StartsWith('#'))
                {
                    File.Copy(file, Path.Combine(gameDirectory,
                        Helpers.StringHelper.OmitFileNamePrefix(relativePath, '#')), true);

                    continue;
                }

                // Check if this file is a merge archive.
                if (fileName.StartsWith('+') && Path.GetExtension(file) == ".arc")
                {
                    var baseArchive  = Database.LoadArchive(Helpers.StringHelper.OmitFileNamePrefix(relativePath, '+'));
                    var mergeArchive = new U8Archive(file, ReadMode.IndexOnly);

                    if (baseArchive == null)
                        throw new Exception("Failed to merge archives as the base archive returned null.");

                    // Merge the two archives together.
                    ArchiveHelper.MergeWith(baseArchive.Root, mergeArchive.Root);

                    continue;
                }

                // Check if this file exists in the game directory.
                if (File.Exists(absolutePath))
                {
                    IOHelper.Backup(absolutePath);

                    File.Copy(file, absolutePath, true);

                    /* Remove the archive from the merge list, 
                       as it has been overwritten entirely. */
                    Database.Archives!.Remove(absolutePath);
                }
            }

            foreach (var patchName in Patches)
            {
                // Check if this patch should be installed.
                if (patchName.StartsWith('+'))
                {
                    var patchPath = Path.Combine(modDirectory, patchName.TrimStart('+'));

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
                if (patchName.StartsWith('-'))
                {
                    // TODO
                }
            }

            State = InstallState.Installed;
        }

        public void Uninstall()
        {
            var gameDirectory = App.Settings.GetGameDirectory();
            var modDirectory  = Path.GetDirectoryName(Location);

            if (string.IsNullOrEmpty(gameDirectory) || string.IsNullOrEmpty(modDirectory))
                return;

            State = InstallState.Uninstalling;

            // Remove custom files.
            foreach (var file in Directory.GetFiles(modDirectory, "#*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(modDirectory, file);
                var originalPath = Path.Combine(gameDirectory, Helpers.StringHelper.OmitFileNamePrefix(relativePath, '#'));

                if (!File.Exists(originalPath))
                    continue;

                File.Delete(originalPath);
            }

            State = InstallState.Idle;
        }
    }
}

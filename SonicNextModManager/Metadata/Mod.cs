using Marathon.Formats.Archive;
using Marathon.Helpers;
using Marathon.IO;
using SonicNextModManager.Helpers;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
        public ObservableCollection<string> Patches { get; set; } = new();

        public Mod() { }

        public Mod(string in_file)
        {
            Mod mod = JsonConvert.DeserializeObject<Mod>(File.ReadAllText(in_file));

            Title       = mod.Title;
            Author      = mod.Author;
            Platform    = mod.Platform;
            Date        = mod.Date;
            Description = mod.Description;
            Location    = in_file;
            Version     = mod.Version;
            Patches     = mod.Patches;

            // Get single thumbnail and use that as the path.
            if (DirectoryHelper.Contains(Path.GetDirectoryName(in_file), "thumbnail.*", out string out_thumbnail))
                mod.Thumbnail = out_thumbnail;
        }

        public static Mod Parse(string in_file)
        {
            return new Mod(in_file);
        }

        public void Write(Mod in_mod, string in_file)
            => File.WriteAllText(in_file, JsonConvert.SerializeObject(in_mod, Formatting.Indented));

        public void Write(string in_file)
            => Write(this, in_file);

        public void Write()
            => Write(this, Location);

        public async Task Install()
        {
            var gameDirectory = App.Settings.GetGameDirectory();
            var modDirectory = Path.GetDirectoryName(Location);

            State = InstallState.Installing;

            // Loop through each file in this mod.
            foreach (string file in Directory.GetFiles(modDirectory, "*", SearchOption.AllDirectories))
            {
                var fileName = Path.GetFileName(file);

                // Skip mod metadata.
                if (fileName == "mod.json" ||
                    Path.GetExtension(fileName) == ".mlua" ||
                    Patches.Any(x => x.Contains(Path.GetFileName(file))) ||
                    Path.GetFileNameWithoutExtension(file) == "thumbnail")
                {
                    continue;
                }

                var relativePath = Path.GetRelativePath(modDirectory, file);
                var absolutePath = Path.Combine(gameDirectory, relativePath);

                // Check if this file is a custom one.
                if (fileName.StartsWith('#'))
                {
                    // Copy this file to the game directory, removing the # symbol.
                    // TODO: remove # symbol safely by file name.
                    File.Copy(file, Path.Combine(gameDirectory, relativePath.Replace("#", "")), true);

                    // Continue to the next file rather than running the rest of this loop.
                    continue;
                }

                // Check if this file is a merge one.
                if (fileName.StartsWith('+') && Path.GetExtension(file) == ".arc")
                {
                    // Load the base game archive.
                    U8Archive baseArchive = await Task.Run(() => Database.ReadArchive(Path.Combine(gameDirectory, relativePath.Replace("+", ""))));
                    
                    // Load our mod archive.
                    U8Archive mergeArchive = new(file, ReadMode.IndexOnly);

                    // Merge the two archives together.
                    ArchiveHelper.MergeWith(baseArchive.Root, mergeArchive.Root);

                    // Continue to the next file rather than running the rest of this loop.
                    continue;
                }

                // Check if this file exists in the game directory.
                if (File.Exists(absolutePath))
                {
                    // If we haven't already, take a backup of the original file.
                    IOHelper.Backup(absolutePath);

                    // Copy this file to the game directory.
                    File.Copy(file, absolutePath, true);

                    // If we've previously loaded this file for a merge or patch, then erase it from the list so we don't overwrite this with an old version.
                    // TODO: Show a notification that some mods had their files replaced maybe?
                    if (Database.Archives.ContainsKey(absolutePath))
                        Database.Archives.Remove(absolutePath);
                }
            }

            foreach (var patchName in Patches)
            {
                if (patchName.StartsWith('+'))
                {
                    var patchPath = Path.Combine(modDirectory, patchName.TrimStart('+'));

                    if (!File.Exists(patchPath))
                    {
                        patchPath = Path.Combine(App.Directories["Patches"], patchName);

                        if (!Directory.Exists(patchPath))
                            continue;

                        patchPath = Path.Combine(patchPath, "patch.lua");
                    }

                    var patch = Patch.Parse(patchPath);

                    patch.Install();
                }

                if (patchName.StartsWith('-'))
                {
                    // TODO
                }
            }

            State = InstallState.Installed;
        }

        public async Task Uninstall()
        {
            var gameDirectory = App.Settings.GetGameDirectory();
            var modDirectory = Path.GetDirectoryName(Location);

            State = InstallState.Uninstalling;

            foreach (string file in Directory.GetFiles(gameDirectory, "*.06mm_backup", SearchOption.AllDirectories))
                File.Move(file, file.Replace(".06mm_backup", ""), true);

            foreach (string file in Directory.GetFiles(modDirectory, "#*.*", SearchOption.AllDirectories))
            {
                string relativePath = file.Replace(modDirectory, "");
                string originalFilePath = $@"{gameDirectory}{relativePath.Replace("#", "")}";

                // If this file exists in the game's directory, then delete it.
                if (File.Exists(originalFilePath))
                    File.Delete(originalFilePath);
            }

            State = InstallState.Idle;
        }
    }
}

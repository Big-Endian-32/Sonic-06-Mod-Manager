using Marathon.IO;
using SonicNextModManager.Helpers;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SonicNextModManager
{
    public class Mod : Metadata
    {
        /// <summary>
        /// The version string for this mod.
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// A collection of patches required by this mod.
        /// </summary>
        public ObservableCollection<string> Patches { get; set; } = new();

        public Mod Parse(string file)
        {
            Mod metadata = JsonConvert.DeserializeObject<Mod>(File.ReadAllText(file));

            // Set metadata path.
            metadata.Path = file;

            // Get single thumbnail and use that as the path.
            if (DirectoryExtensions.Contains(System.IO.Path.GetDirectoryName(file), "thumbnail.*", out string thumbnail))
                metadata.Thumbnail = thumbnail;

            return metadata;
        }

        public void Write(Mod mod, string file)
            => File.WriteAllText(file, JsonConvert.SerializeObject(mod, Formatting.Indented));

        public void Write(string file)
            => Write(this, file);

        public void Write()
			=> Write(this, Path);

        public async Task Install(string gameDirectory)
        {
            // Get this mod's directory.
            string modDirectory = System.IO.Path.GetDirectoryName(Path);

            // Loop through each file in this mod.
            foreach (string modFile in Directory.GetFiles(modDirectory, "*.*", SearchOption.AllDirectories))
            {
                // Skip the mod json and thumbnail.
                // TODO: Skip patch stuff as well once the new system is in place.
                if (System.IO.Path.GetFileName(modFile) == "mod.json" ||
                    System.IO.Path.GetFileName(modFile) == "patch.mlua" ||
                    System.IO.Path.GetFileNameWithoutExtension(modFile) == "thumbnail")
                    continue;

                // Get the relative path to this file.
                string relativePath = modFile.Replace(modDirectory, "");

                // Check if this file is a custom one.
                if (System.IO.Path.GetFileName(modFile).StartsWith('#'))
                {
                    // Copy this file to the game directory, removing the # symbol.
                    File.Copy(modFile, $@"{gameDirectory}{relativePath.Replace("#", "")}", true);

                    // Continue to the next file rather than running the rest of this loop.
                    continue;
                }

                // Check if this file is a merge one.
                // TODO: Safe guard against other file types? While this should only be used on archives I wouldn't put it past someone to screw this up.
                if (System.IO.Path.GetFileName(modFile).StartsWith('+'))
                {
                    // Load the base game archive.
                    Marathon.Formats.Archive.U8Archive baseArchive = await Task.Run(() => ArchiveHelper.ReadArchive($@"{gameDirectory}{relativePath.Replace("+", "")}"));
                    
                    // Load our mod archive.
                    Marathon.Formats.Archive.U8Archive mergeArchive = new(modFile, ReadMode.IndexOnly);

                    // Merge the two archives together.
                    Marathon.Helpers.ArchiveHelper.MergeWith(baseArchive.Root, mergeArchive.Root);

                    // Continue to the next file rather than running the rest of this loop.
                    continue;
                }

                // Check if this file exists in the game directory.
                if (File.Exists($@"{gameDirectory}{relativePath}"))
                {
                    // If we haven't already, take a backup of the original file.
                    if (!File.Exists($@"{gameDirectory}{relativePath}.06mm_backup"))
                        File.Move($@"{gameDirectory}{relativePath}", $@"{gameDirectory}{relativePath}.06mm_backup");

                    // Copy this file to the game directory.
                    File.Copy(modFile, $@"{gameDirectory}{relativePath}", true);

                    // If we've previously loaded this file for a merge or patch, then erase it from the list so we don't overwrite this with an old version.
                    // TODO: Show a notification that some mods had their files replaced maybe?
                    if (App.Archives.ContainsKey($@"{gameDirectory}{relativePath}"))
                        App.Archives.Remove($@"{gameDirectory}{relativePath}");
                }
            }
        }

        public async Task Uninstall(string gameDirectory)
        {
            // Get this mod's directory.
            string modDirectory = System.IO.Path.GetDirectoryName(Path);

            // Loop through each additive file in this mod.
            foreach (string modFile in Directory.GetFiles(modDirectory, "#*.*", SearchOption.AllDirectories))
            {
                // Get the relative path to this file.
                string relativePath = modFile.Replace(modDirectory, "");

                // If this file exists in the game's directory, then delete it.
                if (File.Exists($@"{gameDirectory}{relativePath.Replace("#", "")}"))
                    File.Delete($@"{gameDirectory}{relativePath.Replace("#", "")}");
            }
        }
    }

    [ValueConversion(typeof(string), typeof(int))]
    public class Thumbnail2WidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => string.IsNullOrEmpty((string)value) ? 0 : 320;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

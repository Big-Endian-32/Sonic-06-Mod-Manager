using Marathon.Helpers;

namespace SonicNextModManager.SiS
{
    public class MetadataConverter
    {
        /// <summary>
        /// Converts older metadata to the new format.
        /// </summary>
        /// <param name="in_file">File to parse metadata from.</param>
        public static Mod Convert(string in_file)
        {
            // Set up a new mod entry and convert values from the old INI file.
            Mod mod = new()
            {
                Title = DeserialiseKey(in_file, "Title"),
                Version = DeserialiseKey(in_file, "Version"),
                Date = DeserialiseKey(in_file, "Date"),
                Author = DeserialiseKey(in_file, "Author"),
                Platform = PlatformConverter.Convert(DeserialiseKey(in_file, "Platform")),
                Description = DeserialiseKey(in_file, "Description"),
                Path = in_file
            };

            // Deserialise the old INI keys.
            var isMerge = bool.Parse(DeserialiseKey(in_file, "Merge"));
            var requiredPatches = DeserialiseKey(in_file, "RequiredPatches").Split(',', StringSplitOptions.RemoveEmptyEntries);
            var readOnly = DeserialiseKey(in_file, "Read-only").Split(',', StringSplitOptions.RemoveEmptyEntries);
            var custom = DeserialiseKey(in_file, "Custom").Split(',', StringSplitOptions.RemoveEmptyEntries);

            // Loop through each patch in the requiredPatches CSV string and add it to our new list, appending a + to indicate its required in the new system.
            foreach (var patch in requiredPatches)
                mod.Patches.Add($"+{Path.GetFileNameWithoutExtension(patch)}");

            foreach (var file in Directory.EnumerateFiles(Path.GetDirectoryName(in_file), "*", SearchOption.AllDirectories))
            {
                // Get this file's filename.
                var fileName = Path.GetFileName(file);

                // Check for this file in the custom CSV string.
                if (custom.Contains(fileName))
                {
                    // Prepend a # to this file's name and abort the rest of the loop.
                    File.Move(file, StringHelper.ReplaceFilename(file, $"#{fileName}"));
                    continue;
                }

                // Check for this file in the readOnly CSV string. Also check that it's an archive, as this should only ever have been used on them.
                if (isMerge && !readOnly.Contains(fileName) && Path.GetExtension(fileName) == ".arc")
                    File.Move(file, StringHelper.ReplaceFilename(file, $"+{fileName}"));
            }

            // Write the mod's configuration to a mod.json file.
            mod.Write(mod, StringHelper.ReplaceFilename(in_file, "mod.json"));

            // Delete the old, now useless, mod.ini file.
            File.Delete(in_file);

            // Return our new, converted mod entry.
            return mod;
        }

        /// <summary>
        /// A dirty way of deserialising an INI key.
        /// <para><see href="https://github.com/hyperbx/SonicNextModManager/blob/Project-Rush/Sonic-06-Mod-Manager/src/UnifySerialisers.cs#L48">Learn more...</see></para>
        /// </summary>
        /// <param name="file">Input file.</param>
        /// <param name="key">Key to deserialise.</param>
        private static string DeserialiseKey(string file, string key)
        {
            string line, entryValue = string.Empty;

            using (StreamReader streamReader = new(file))
            {
                try
                {
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (line.Split('=')[0] == key)
                        {
                            entryValue = line.Substring(line.IndexOf("=") + 2);
                            entryValue = entryValue.Remove(entryValue.Length - 1);
                        }
                    }
                }
                catch
                {
                    // Ignored...
                }
            }

            return entryValue;
        }
    }
}

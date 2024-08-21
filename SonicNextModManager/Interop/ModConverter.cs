using Marathon.Helpers;
using SonicNextModManager.Metadata;

namespace SonicNextModManager.Interop
{
    public class ModConverter
    {
        /// <summary>
        /// Converts an old mod to the new format.
        /// </summary>
        /// <param name="in_file">The path to the INI file to parse.</param>
        public static Mod Convert(string in_file)
        {
            Mod mod = new()
            {
                Title = DeserialiseKey(in_file, "Title"),
                Version = DeserialiseKey(in_file, "Version"),
                Date = DeserialiseKey(in_file, "Date"),
                Author = DeserialiseKey(in_file, "Author"),
                Platform = PlatformConverter.Convert(DeserialiseKey(in_file, "Platform")),
                Description = DeserialiseKey(in_file, "Description"),
                Location = in_file
            };

            var isMerge = bool.Parse(DeserialiseKey(in_file, "Merge"));
            var requiredPatches = DeserialiseKey(in_file, "RequiredPatches").Split(',', StringSplitOptions.RemoveEmptyEntries);
            var readOnly = DeserialiseKey(in_file, "Read-only").Split(',', StringSplitOptions.RemoveEmptyEntries);
            var custom = DeserialiseKey(in_file, "Custom").Split(',', StringSplitOptions.RemoveEmptyEntries);

            // Mark required patches with a prefix char and change the extension to *.lua.
            foreach (var patch in requiredPatches)
                mod.Patches.Add($"{Database.AppendChar}{Path.ChangeExtension(Path.GetFileName(patch), ".lua")}");

            foreach (var file in Directory.EnumerateFiles(Path.GetDirectoryName(in_file)!, "*", SearchOption.AllDirectories))
            {
                var fileName = Path.GetFileName(file);

                // Check for this file in the custom CSV string.
                if (custom.Contains(fileName))
                {
                    // Mark this file as custom with a prefix char.
                    File.Move(file, StringHelper.ReplaceFilename(file, $"{Database.CustomChar}{fileName}"));
                    continue;
                }

                /* Check for this file in the read-only CSV string.
                   Also check that it's an archive, as this should
                   only ever have been used on them. */
                if (isMerge && !readOnly.Contains(fileName) && Path.GetExtension(fileName) == ".arc")
                {
                    // Mark this archive as an append archive with a prefix char.
                    File.Move(file, StringHelper.ReplaceFilename(file, $"{Database.AppendChar}{fileName}"));
                }
            }

            Mod.Write(mod, StringHelper.ReplaceFilename(in_file, "mod.json"));

            File.Delete(in_file);

            return mod;
        }

        /// <summary>
        /// A dirty way of deserialising an INI key.
        /// <para><see href="https://github.com/hyperbx/SonicNextModManager/blob/Project-Rush/Sonic-06-Mod-Manager/src/UnifySerialisers.cs#L48">Learn more...</see></para>
        /// </summary>
        /// <param name="in_file">The path to the INI to parse.</param>
        /// <param name="in_key">The key to deserialise.</param>
        private static string DeserialiseKey(string in_file, string in_key)
        {
            string line, value = string.Empty;

            using (var sr = new StreamReader(in_file))
            {
                try
                {
                    while ((line = sr.ReadLine()!) != null)
                    {
                        if (line.Split('=')[0] == in_key)
                        {
                            value = line.Substring(line.IndexOf("=") + 2);
                            value = value.Remove(value.Length - 1);
                        }
                    }
                }
                catch
                {
                    // Ignored...
                }
            }

            return value;
        }
    }
}

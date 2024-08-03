using System.Collections.ObjectModel;
using Marathon.Helpers;
using SonicNextModManager.IO;

namespace SonicNextModManager
{
    public class Patch : Metadata
    {
        /// <summary>
        /// The category that represents this patch.
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// The short description for this patch.
        /// </summary>
        public string? Blurb { get; set; }

        /// <summary>
        /// The name of the first function to call in the Lua script - if null, the script in its entirety will be executed.
        /// </summary>
        public string? Function { get; set; }

        /// <summary>
        /// A collection of described declared variables used in the Lua script - used for configuration.
        /// </summary>
        public ObservableCollection<Declaration> Declarations { get; set; } = new();

        /// <summary>
        /// Returns patch metadata parsed from the input file.
        /// </summary>
        /// <param name="file">Lua script to pull metadata from.</param>
        public Patch Parse(string file)
        {
            Patch metadata = JsonConvert.DeserializeObject<Patch>(File.ReadAllText(file));

            // Set metadata path.
            metadata.Path = file;

            return metadata;
        }

        public void Write(string file)
            => File.WriteAllText(file, JsonConvert.SerializeObject(this, Formatting.Indented));

        public void Write()
            => Write(Path);

        public void Install()
        {
            // Add game executable symbol.
            Patcher.AddSymbol("Executable", App.Settings.Path_GameExecutable);

            // Initialise script interpreter.
            Script interpreter = new();

            // Run script.
            interpreter.Initialise();
            interpreter.DoFile(Marathon.Helpers.StringHelper.ReplaceFilename(Path, "patch.lua"));

            if (string.IsNullOrEmpty(Function))
                return;

            // Call the user-specified function.
            interpreter.Call(interpreter.Globals[Function]);
        }
    }
}

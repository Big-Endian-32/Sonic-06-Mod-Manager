using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Extensions;
using SonicNextModManager.Lua.Wrappers;
using SonicNextModManager.Lua.Wrappers.Archive;
using SonicNextModManager.Metadata;

namespace SonicNextModManager.Lua.Callback
{
    public class IOFunctions
    {
        /// <summary>
        /// Loads a file with helper functions.
        /// </summary>
        /// <param name="in_path">The path to the file to load.</param>
        [LuaCallback]
        public static DynValue Open(Script L, string in_path)
        {
            in_path = L.GetGlobal<string>(in_path);

            if (Path.GetExtension(in_path) == ".arc")
                return UserData.Create(new U8ArchiveWrapper(in_path));

            Database.Backup(in_path);

            return UserData.Create(new FileWrapper(in_path));
        }

        /// <summary>
        /// Copies a file to the specified location.
        /// </summary>
        /// <param name="in_source">The path to the source file.</param>
        /// <param name="in_destination">The path to copy to.</param>
        [LuaCallback]
        public static bool Copy(Script L, string in_source, string in_destination, bool in_isOverwrite = true)
        {
            var scriptName = L.GetGlobal<string>("ScriptName");
            var source = Path.Combine(L.GetGlobal<string>("Work"), in_source);

            if (!File.Exists(source))
            {
                source = in_source;

                LoggerService.Warning("Copying files from outside of the work directory!", scriptName);

                if (!File.Exists(source))
                {
                    LoggerService.Error($"Could not find source file: {source}", scriptName);
                    return false;
                }
            }

            Database.Backup(in_destination);

            File.Copy(source, in_destination, in_isOverwrite);

            // Create marker file for uninstallation.
            File.Open($"{in_destination}{Database.CustomExt}", FileMode.Create);

            return true;
        }
    }
}

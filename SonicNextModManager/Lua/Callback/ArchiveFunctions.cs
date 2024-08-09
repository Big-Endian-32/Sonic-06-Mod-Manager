using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Wrappers.Archive;

namespace SonicNextModManager.Lua.Callback
{
    public class ArchiveFunctions
    {
        [LuaCallback]
        public static DynValue LoadArchive(string in_path)
        {
            return UserData.Create(new U8ArchiveWrapper(in_path));
        }
    }
}

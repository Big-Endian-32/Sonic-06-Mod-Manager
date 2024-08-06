using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Wrappers.Text;

namespace SonicNextModManager.Lua.Callback
{
    public class TextFunctions
    {
        [LuaCallback]
        public static DynValue LoadMessageTable(string in_path)
        {
            var file = ArchiveHelper.GetArchiveFile(in_path);

            if (file == null)
                return DynValue.Nil;

            return UserData.Create(new MessageTableWrapper(file));
        }
    }
}

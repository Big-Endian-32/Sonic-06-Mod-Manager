using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Wrappers;
using SonicNextModManager.Lua.Wrappers.Text;

namespace SonicNextModManager.Lua.Callback
{
    public class TextFunctions
    {
        [LuaCallback]
        public static DynValue LoadMessageTable(string in_path)
        {
            return MarathonWrapper.RegisterWrapperToArchiveFile<MessageTableWrapper>(in_path);
        }
    }
}

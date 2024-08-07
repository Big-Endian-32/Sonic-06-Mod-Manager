using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Wrappers;
using SonicNextModManager.Lua.Wrappers.Script;

namespace SonicNextModManager.Lua.Callback
{
    public class ScriptFunctions
    {
        [LuaCallback]
        public static DynValue LoadLuaScript(string in_path)
        {
            if (Path.GetExtension(in_path) == ".lua")
                in_path = Path.ChangeExtension(in_path, ".lub");

            return MarathonWrapper.RegisterWrapperToArchiveFile<LuaBinaryWrapper>(in_path);
        }
    }
}

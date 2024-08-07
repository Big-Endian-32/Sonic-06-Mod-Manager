using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Wrappers;
using SonicNextModManager.Lua.Wrappers.Package;

namespace SonicNextModManager.Lua.Callback
{
    public class PackageFunctions
    {
        [LuaCallback]
        public static DynValue LoadAssetPackage(string in_path)
        {
            return MarathonWrapper.RegisterWrapperToArchiveFile<AssetPackageWrapper>(in_path);
        }
    }
}

using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Wrappers.Package;

namespace SonicNextModManager.Lua.Callback
{
    public class PackageFunctions
    {
        [LuaCallback]
        public static DynValue LoadAssetPackage(string in_path)
        {
            var file = ArchiveHelper.GetArchiveFile(in_path);

            if (file == null)
                return DynValue.Nil;

            return UserData.Create(new AssetPackageWrapper(file));
        }
    }
}

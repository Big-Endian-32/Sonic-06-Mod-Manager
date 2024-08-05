using Marathon.Formats.Archive;
using Marathon.Formats.Package;
using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Interfaces;

namespace SonicNextModManager.Lua.Wrappers.Package
{
    [LuaUserData]
    public class AssetPackageWrapper : ILuaUserDataDescriptor
    {
        private U8ArchiveFile _file;
        private AssetPackage _assetPackage;

        public AssetPackageWrapper() { }

        public AssetPackageWrapper(U8ArchiveFile in_file)
        {
            _file = in_file;
            _assetPackage = IOHelper.LoadMarathonTypeFromBuffer<AssetPackage>(_file.Data);
        }

        public void Register()
        {
            UserData.RegisterType<AssetPackage>();
            UserData.RegisterType<AssetType>();
            UserData.RegisterType<AssetFile>();
        }

        public AssetTypeWrapper GetType(string in_name)
        {
            return new AssetTypeWrapper(_assetPackage.Types.Where(x => x.Name == in_name).Single());
        }

        public AssetFile? GetFile(string in_path)
        {
            var paths = in_path.Replace('\\', '/').Split('/');

            if (paths.Length < 2)
                return null;

            return GetType(paths[0]).GetFile(paths[1]);
        }

        public void DeleteFile(string in_path)
        {
            var paths = in_path.Replace('\\', '/').Split('/');

            if (paths.Length < 2)
                return;

            GetType(paths[0]).DeleteFile(paths[1]);
        }

        public void Save()
        {
            _file.Data = IOHelper.GetMarathonTypeBuffer(_assetPackage);
        }
    }

    [LuaUserData]
    public class AssetTypeWrapper
    {
        private AssetType _assetType;

        public AssetTypeWrapper() { }

        public AssetTypeWrapper(AssetType in_assetType)
        {
            _assetType = in_assetType;
        }

        public AssetFile GetFile(string in_name)
        {
            return _assetType.Files.Where(x => x.Name == in_name).Single();
        }

        public void DeleteFile(string in_name)
        {
            _assetType.Files.Remove(GetFile(in_name));
        }
    }
}

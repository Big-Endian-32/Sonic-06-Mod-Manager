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

        public AssetCategoryWrapper this[string in_name]
        {
            get => GetCategory(in_name);
        }

        public void Register()
        {
            UserData.RegisterType<AssetPackage>();
            UserData.RegisterType<AssetCategory>();
            UserData.RegisterType<AssetFile>();
        }

        public AssetCategoryWrapper GetCategory(string in_name)
        {
            return new AssetCategoryWrapper(_assetPackage.Categories.Where(x => x.Name == in_name).Single());
        }

        public AssetFile? GetFile(string in_path)
        {
            var paths = in_path.Replace('\\', '/').Split('/');

            if (paths.Length < 2)
                return null;

            return GetCategory(paths[0]).GetFile(paths[1]);
        }

        public void DeleteFile(string in_path)
        {
            var paths = in_path.Replace('\\', '/').Split('/');

            if (paths.Length < 2)
                return;

            GetCategory(paths[0]).DeleteFile(paths[1]);
        }

        public void Save()
        {
            _file.Data = IOHelper.GetMarathonTypeBuffer(_assetPackage);
        }
    }

    [LuaUserData]
    public class AssetCategoryWrapper
    {
        private AssetCategory _assetCategory;

        public AssetCategoryWrapper() { }

        public AssetCategoryWrapper(AssetCategory in_assetType)
        {
            _assetCategory = in_assetType;
        }

        public AssetFile this[string in_name]
        {
            get => GetFile(in_name);
        }

        public AssetFile GetFile(string in_name)
        {
            return _assetCategory.Files.Where(x => x.Name == in_name).Single();
        }

        public void DeleteFile(string in_name)
        {
            _assetCategory.Files.Remove(GetFile(in_name));
        }
    }
}

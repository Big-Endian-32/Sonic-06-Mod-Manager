using Marathon.Formats.Archive;
using Marathon.Formats.Package;
using SonicNextModManager.Extensions;
using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Interfaces;

namespace SonicNextModManager.Lua.Wrappers.Package
{
    [LuaUserData]
    public class AssetPackageWrapper : MarathonWrapper, ILuaUserDataDescriptor
    {
        private AssetPackage _assetPackage;

        public AssetPackageWrapper() { }

        public AssetPackageWrapper(U8ArchiveFile in_file) : base(in_file)
        {
            _assetPackage = IOHelper.LoadMarathonTypeFromBuffer<AssetPackage>(File.Data);
        }

        public AssetCategoryWrapper this[string in_name]
        {
            get => GetCategory(in_name);
        }

        public void Register(MoonSharp.Interpreter.Script L)
        {
            L.RegisterType<AssetPackage>();
            L.RegisterType<AssetCategory>();
            L.RegisterType<AssetFile>();
        }

        public void AddCategory(string in_name)
        {
            _assetPackage.Categories.Add(new(in_name));
        }

        public void RemoveCategory(string in_name)
        {
            _assetPackage.Categories.RemoveAll(x => x.Name == in_name);
        }

        public AssetCategoryWrapper GetCategory(string in_name)
        {
            return new AssetCategoryWrapper(_assetPackage.Categories.Where(x => x.Name == in_name).FirstOrDefault()!);
        }

        public AssetCategoryWrapper[] GetCategories()
        {
            return _assetPackage.Categories.Select(x => new AssetCategoryWrapper(x)).ToArray();
        }

        public void Save()
        {
            Save(_assetPackage);
        }
    }

    [LuaUserData]
    public class AssetCategoryWrapper
    {
        private AssetCategory _assetCategory;

        public AssetCategoryWrapper() { }

        public AssetCategoryWrapper(AssetCategory in_assetCategory)
        {
            _assetCategory = in_assetCategory;
        }

        public AssetFile this[string in_name]
        {
            get => GetFile(in_name);
        }

        public void AddFile(string in_name, string in_path)
        {
            _assetCategory.Files.Add(new(in_name, in_path));
        }

        public void RemoveFile(string in_name)
        {
            _assetCategory.Files.RemoveAll(x => x.Name == in_name);
        }

        public AssetFile GetFile(string in_name)
        {
            return _assetCategory.Files.Where(x => x.Name == in_name).FirstOrDefault()!;
        }

        public AssetFile[] GetFiles()
        {
            return [.. _assetCategory.Files];
        }
    }
}

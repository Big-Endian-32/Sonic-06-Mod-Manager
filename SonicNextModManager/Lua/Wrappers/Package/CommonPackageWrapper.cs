using Marathon.Formats.Archive;
using Marathon.Formats.Package;
using SonicNextModManager.Extensions;
using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Interfaces;

namespace SonicNextModManager.Lua.Wrappers.Package
{
    [LuaUserData]
    public class CommonPackageWrapper : MarathonWrapper, ILuaUserDataDescriptor
    {
        private CommonPackage _commonPackage;

        public CommonPackageWrapper() { }

        public CommonPackageWrapper(U8ArchiveFile in_file) : base(in_file)
        {
            _commonPackage = IOHelper.LoadMarathonTypeFromBuffer<CommonPackage>(File.Data);
        }

        public CommonObject this[string in_name]
        {
            get => GetObject(in_name);
        }

        public void Register(MoonSharp.Interpreter.Script L)
        {
            L.RegisterType<CommonObject>();
        }

        public CommonObject GetObject(string in_name)
        {
            return _commonPackage.Objects.Where(x => x.PropName == in_name).FirstOrDefault()!;
        }

        public CommonObject[] GetObjects()
        {
            return [.. _commonPackage.Objects];
        }

        public void SetObject(DynValue in_value)
        {
            var @object = in_value.ParseClassFromDynValue<CommonObject>();
            var index = _commonPackage.Objects.FindIndex(x => x.PropName == @object.PropName);

            if (index == -1)
            {
                _commonPackage.Objects.Add(@object);
            }
            else
            {
                _commonPackage.Objects[index] = @object;
            }
        }

        public void Save()
        {
            Save(_commonPackage);
        }
    }
}

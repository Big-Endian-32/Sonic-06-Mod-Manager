using Marathon.Formats.Archive;
using Marathon.Formats.Package;
using SonicNextModManager.Extensions;
using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Interfaces;

namespace SonicNextModManager.Lua.Wrappers.Package
{
    [LuaUserData]
    public class ShotPackageWrapper : MarathonWrapper, ILuaUserDataDescriptor
    {
        private ShotPackage _shotPackage;

        public ShotPackageWrapper() { }

        public ShotPackageWrapper(U8ArchiveFile in_file) : base(in_file)
        {
            _shotPackage = IOHelper.LoadMarathonTypeFromBuffer<ShotPackage>(File.Data);
        }

        public ShotParameter this[string in_name]
        {
            get => GetParameter(in_name);
        }

        public void Register(MoonSharp.Interpreter.Script L)
        {
            L.RegisterType<ShotParameter>();
        }

        public ShotParameter GetParameter(string in_name)
        {
            return _shotPackage.Parameters.Where(x => x.Name == in_name).FirstOrDefault()!;
        }

        public ShotParameter[] GetParameters()
        {
            return [.. _shotPackage.Parameters];
        }

        public void SetParameter(DynValue in_table)
        {
            var shotParameter = in_table.ParseClassFromDynValue<ShotParameter>();
            var index = _shotPackage.Parameters.FindIndex(x => x.Name == shotParameter.Name);

            if (index == -1)
            {
                _shotPackage.Parameters.Add(shotParameter);
            }
            else
            {
                _shotPackage.Parameters[index] = shotParameter;
            }
        }

        public void Save()
        {
            Save(_shotPackage);
        }
    }
}

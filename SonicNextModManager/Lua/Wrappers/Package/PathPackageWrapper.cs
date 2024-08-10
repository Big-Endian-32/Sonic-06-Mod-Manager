using Marathon.Formats.Archive;
using Marathon.Formats.Package;
using SonicNextModManager.Extensions;
using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Interfaces;

namespace SonicNextModManager.Lua.Wrappers.Package
{
    [LuaUserData]
    public class PathPackageWrapper : MarathonWrapper, ILuaUserDataDescriptor
    {
        private PathPackage _pathPackage;

        public PathPackageWrapper() { }

        public PathPackageWrapper(U8ArchiveFile in_file) : base(in_file)
        {
            _pathPackage = IOHelper.LoadMarathonTypeFromBuffer<PathPackage>(File.Data);
        }

        public void Register(MoonSharp.Interpreter.Script L)
        {
            L.RegisterType<PathObject>();
        }

        public PathObject GetPathObject(string in_name)
        {
            return _pathPackage.PathObjects.Where(x => x.Name == in_name).FirstOrDefault()!;
        }

        public PathObject[] GetPathObjects()
        {
            return [.. _pathPackage.PathObjects];
        }

        public void SetPathObject(DynValue in_table)
        {
            var pathObject = in_table.ParseClassFromDynValue<PathObject>();
            var index = _pathPackage.PathObjects.FindIndex(x => x.Name == pathObject.Name);

            if (index == -1)
            {
                _pathPackage.PathObjects.Add(pathObject);
            }
            else
            {
                _pathPackage.PathObjects[index] = pathObject;
            }
        }

        public void Save()
        {
            Save(_pathPackage);
        }
    }
}

using Marathon.Formats.Archive;
using Marathon.Formats.Package;
using SonicNextModManager.Extensions;
using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Interfaces;

namespace SonicNextModManager.Lua.Wrappers.Package
{
    [LuaUserData]
    public class ScriptPackageWrapper : MarathonWrapper, ILuaUserDataDescriptor
    {
        private ScriptPackage _scriptPackage;

        public ScriptPackageWrapper() { }

        public ScriptPackageWrapper(U8ArchiveFile in_file) : base(in_file)
        {
            _scriptPackage = IOHelper.LoadMarathonTypeFromBuffer<ScriptPackage>(File.Data);
        }

        public void Register(MoonSharp.Interpreter.Script L)
        {
            L.RegisterType<ScriptParameter>();
        }

        public ScriptParameter GetParameter(string in_name)
        {
            return _scriptPackage.Parameters.Where(x => x.Name == in_name).FirstOrDefault()!;
        }

        public ScriptParameter[] GetParameters()
        {
            return [.. _scriptPackage.Parameters];
        }

        public void AddParameter(DynValue in_table)
        {
            _scriptPackage.Parameters.Add(in_table.ParseClassFromDynValue<ScriptParameter>());
        }

        public void Save()
        {
            Save(_scriptPackage);
        }
    }
}

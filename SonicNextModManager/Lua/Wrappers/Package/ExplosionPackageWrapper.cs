using Marathon.Formats.Archive;
using Marathon.Formats.Package;
using SonicNextModManager.Extensions;
using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Interfaces;

namespace SonicNextModManager.Lua.Wrappers.Package
{
    [LuaUserData]
    public class ExplosionPackageWrapper : MarathonWrapper, ILuaUserDataDescriptor
    {
        private ExplosionPackage _explosionPackage;

        public ExplosionPackageWrapper() { }

        public ExplosionPackageWrapper(U8ArchiveFile in_file) : base(in_file)
        {
            _explosionPackage = IOHelper.LoadMarathonTypeFromBuffer<ExplosionPackage>(File.Data);
        }

        public Explosion this[string in_name]
        {
            get => GetExplosion(in_name);
        }

        public void Register(MoonSharp.Interpreter.Script L)
        {
            L.RegisterType<Explosion>();
        }

        public Explosion GetExplosion(string in_name)
        {
            return _explosionPackage.Explosions.Where(x => x.Name == in_name).FirstOrDefault()!;
        }

        public Explosion[] GetExplosions()
        {
            return [.. _explosionPackage.Explosions];
        }

        public void SetExplosion(DynValue in_table)
        {
            var explosion = in_table.ParseClassFromDynValue<Explosion>();
            var index = _explosionPackage.Explosions.FindIndex(x => x.Name == explosion.Name);

            if (index == -1)
            {
                _explosionPackage.Explosions.Add(explosion);
            }
            else
            {
                _explosionPackage.Explosions[index] = explosion;
            }
        }

        public void Save()
        {
            Save(_explosionPackage);
        }
    }
}

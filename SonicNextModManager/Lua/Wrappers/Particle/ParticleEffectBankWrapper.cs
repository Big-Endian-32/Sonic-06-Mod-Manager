using Marathon.Formats.Archive;
using Marathon.Formats.Particle;
using SonicNextModManager.Extensions;
using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Interfaces;

namespace SonicNextModManager.Lua.Wrappers.Particle
{
    [LuaUserData]
    public class ParticleEffectBankWrapper : MarathonWrapper, ILuaUserDataDescriptor
    {
        private ParticleEffectBank _particleContainer;

        public ParticleEffectBankWrapper() { }

        public ParticleEffectBankWrapper(U8ArchiveFile in_file) : base(in_file)
        {
            _particleContainer = IOHelper.LoadMarathonTypeFromBuffer<ParticleEffectBank>(File.Data);
        }

        public ParticleEffect this[string in_name]
        {
            get => GetEffect(in_name);
        }

        public void Register(MoonSharp.Interpreter.Script L)
        {
            L.RegisterType<ParticleEffect>();
            L.RegisterType<ParticleEffectAttributes>();
            L.RegisterType<ParticleEffectProperty>();
            L.RegisterType<ParticleEffectPropertyType>("EParticleEffectPropertyType");
        }

        public ParticleEffect GetEffect(string in_name)
        {
            return _particleContainer.Data.Effects.Where(x => x.Name == in_name).FirstOrDefault()!;
        }

        public ParticleEffect[] GetEffects()
        {
            return [.. _particleContainer.Data.Effects];
        }

        public void Save()
        {
            Save(_particleContainer);
        }
    }
}

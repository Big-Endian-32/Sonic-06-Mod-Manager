using Marathon.Formats.Archive;
using Marathon.Formats.Particle;
using SonicNextModManager.Extensions;
using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Interfaces;

namespace SonicNextModManager.Lua.Wrappers.Particle
{
    [LuaUserData]
    public class ParticleContainerWrapper : MarathonWrapper, ILuaUserDataDescriptor
    {
        private ParticleContainer _particleContainer;

        public ParticleContainerWrapper() { }

        public ParticleContainerWrapper(U8ArchiveFile in_file) : base(in_file)
        {
            _particleContainer = IOHelper.LoadMarathonTypeFromBuffer<ParticleContainer>(File.Data);
        }

        public ParticleAttributes this[string in_name]
        {
            get => GetParticle(in_name);
        }

        public void Register(MoonSharp.Interpreter.Script L)
        {
            L.RegisterType<ParticleAttributes>();
        }

        public ParticleAttributes GetParticle(string in_name)
        {
            return _particleContainer.Data.Particles.Where(x => x.ParticleName == in_name).FirstOrDefault()!;
        }

        public ParticleAttributes[] GetParticles()
        {
            return [.. _particleContainer.Data.Particles];
        }

        public void SetParticle(string in_particleName, string in_effectName, string in_path, uint in_flags)
        {
            if (_particleContainer.Data.Particles.Any(x => x.ParticleName == in_particleName))
            {
                var picture = GetParticle(in_particleName);

                picture.EffectName = in_effectName;
                picture.File = in_path;
                picture.Flags = in_flags;
            }
            else
            {
                _particleContainer.Data.Particles.Add(new(in_particleName, in_effectName, in_path, in_flags));
            }
        }

        public void SetParticle(DynValue in_value)
        {
            var particle = in_value.ParseClassFromDynValue<ParticleAttributes>();
            var index = _particleContainer.Data.Particles.FindIndex(x => x.ParticleName == particle.ParticleName);

            if (index == -1)
            {
                _particleContainer.Data.Particles.Add(particle);
            }
            else
            {
                _particleContainer.Data.Particles[index] = particle;
            }
        }

        public void Save()
        {
            Save(_particleContainer);
        }
    }
}

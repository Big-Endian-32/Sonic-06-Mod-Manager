using Marathon.Formats.Archive;
using Marathon.Formats.Particle;
using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Extensions;
using SonicNextModManager.Lua.Interfaces;

namespace SonicNextModManager.Lua.Wrappers.Particle
{
    [LuaUserData]
    public class ParticleGenerationSystemWrapper : MarathonWrapper, ILuaUserDataDescriptor
    {
        private ParticleGenerationSystem _particleGenerationSystem;

        public ParticleGenerationSystemWrapper() { }

        public ParticleGenerationSystemWrapper(U8ArchiveFile in_file) : base(in_file)
        {
            _particleGenerationSystem = IOHelper.LoadMarathonTypeFromBuffer<ParticleGenerationSystem>(File.Data);
        }

        public void Register(MoonSharp.Interpreter.Script L)
        {
            L.RegisterType<ParticleMaterial>();
            L.RegisterType<BlendMode>("EBlendMode");
        }

        public void AddEffectBank(string in_name)
        {
            _particleGenerationSystem.Data.EffectBanks.Add(in_name);
        }

        public void RemoveEffectBank(string in_name)
        {
            _particleGenerationSystem.Data.EffectBanks.Remove(in_name);
        }

        public string[] GetEffectBanks()
        {
            return [.. _particleGenerationSystem.Data.EffectBanks];
        }

        public void SetEffectBanks(DynValue in_value)
        {
            if (in_value.Type != DataType.Table)
                return;

            _particleGenerationSystem.Data.EffectBanks = in_value.Table.Values.Select(x => x.String).ToList();
        }

        public void AddTextureBank(string in_name)
        {
            _particleGenerationSystem.Data.TextureBanks.Add(in_name);
        }

        public void RemoveTextureBank(string in_name)
        {
            _particleGenerationSystem.Data.TextureBanks.Remove(in_name);
        }

        public string[] GetTextureBanks()
        {
            return [.. _particleGenerationSystem.Data.TextureBanks];
        }

        public void SetTextureBanks(DynValue in_value)
        {
            if (in_value.Type != DataType.Table)
                return;

            _particleGenerationSystem.Data.TextureBanks = in_value.Table.Values.Select(x => x.String).ToList();
        }

        public ParticleMaterial GetMaterial(string in_name)
        {
            return _particleGenerationSystem.Data.Materials.Where(x => x.Name == in_name).FirstOrDefault()!;
        }

        public ParticleMaterial[] GetMaterials()
        {
            return [.. _particleGenerationSystem.Data.Materials];
        }

        public void SetMaterial(string in_name, string in_properties, BlendMode in_blendMode)
        {
            if (_particleGenerationSystem.Data.Materials.Any(x => x.Name == in_name))
            {
                var picture = GetMaterial(in_name);

                picture.Properties = in_properties;
                picture.BlendMode = in_blendMode;
            }
            else
            {
                _particleGenerationSystem.Data.Materials.Add(new(in_name, in_properties, in_blendMode));
            }
        }

        public void SetMaterial(DynValue in_value)
        {
            var material = in_value.ParseClassFromDynValue<ParticleMaterial>();
            var index = _particleGenerationSystem.Data.Materials.FindIndex(x => x.Name == material.Name);

            if (index == -1)
            {
                _particleGenerationSystem.Data.Materials.Add(material);
            }
            else
            {
                _particleGenerationSystem.Data.Materials[index] = material;
            }
        }

        public void Close()
        {
            Close(_particleGenerationSystem);
        }
    }
}

using Marathon.Formats.Archive;
using Marathon.Formats.Particle;
using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Extensions;
using SonicNextModManager.Lua.Interfaces;

namespace SonicNextModManager.Lua.Wrappers.Particle
{
    [LuaUserData]
    public class ParticleTextureBankWrapper : MarathonWrapper, ILuaUserDataDescriptor
    {
        private ParticleTextureBank _particleTextureBank;

        public ParticleTextureBankWrapper() { }

        public ParticleTextureBankWrapper(U8ArchiveFile in_file) : base(in_file)
        {
            _particleTextureBank = IOHelper.LoadMarathonTypeFromBuffer<ParticleTextureBank>(File.Data);
        }

        public ParticleTexture this[string in_name]
        {
            get => GetTexture(in_name);
        }

        public void Register(MoonSharp.Interpreter.Script L)
        {
            L.RegisterType<ParticleTexture>();
        }

        public ParticleTexture GetTexture(string in_name)
        {
            return _particleTextureBank.Data.ParticleTextures.Where(x => x.Name == in_name).FirstOrDefault()!;
        }

        public void SetTexture(string in_name, string in_path, uint in_width, uint in_height)
        {
            if (_particleTextureBank.Data.ParticleTextures.Any(x => x.Name == in_name))
            {
                var picture = GetTexture(in_name);

                picture.Path = in_path;
                picture.Width = in_width;
                picture.Height = in_height;
            }
            else
            {
                _particleTextureBank.Data.ParticleTextures.Add(new(in_name, in_path, in_width, in_height));
            }
        }

        public void SetTexture(DynValue in_value)
        {
            var texture = in_value.ParseClassFromDynValue<ParticleTexture>();
            var index = _particleTextureBank.Data.ParticleTextures.FindIndex(x => x.Name == texture.Name);

            if (index == -1)
            {
                _particleTextureBank.Data.ParticleTextures.Add(texture);
            }
            else
            {
                _particleTextureBank.Data.ParticleTextures[index] = texture;
            }
        }

        public void Close()
        {
            Close(_particleTextureBank);
        }
    }
}

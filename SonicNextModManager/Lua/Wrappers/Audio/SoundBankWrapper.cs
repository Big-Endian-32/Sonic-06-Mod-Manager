using Marathon.Formats.Archive;
using Marathon.Formats.Audio;
using SonicNextModManager.Extensions;
using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Interfaces;

namespace SonicNextModManager.Lua.Wrappers.Audio
{
    [LuaUserData]
    public class SoundBankWrapper : MarathonWrapper, ILuaUserDataDescriptor
    {
        private SoundBank _soundBank;

        public SoundBankWrapper() { }

        public SoundBankWrapper(U8ArchiveFile in_file) : base(in_file)
        {
            _soundBank = IOHelper.LoadMarathonTypeFromBuffer<SoundBank>(File.Data);
        }

        public void Register(MoonSharp.Interpreter.Script L)
        {
            L.RegisterType<Cue>();
        }

        public void RemoveCue(string in_name)
        {
            _soundBank.Data.Cues.RemoveAll(x => x.Name == in_name);
        }

        public Cue GetCue(string in_name)
        {
            return _soundBank.Data.Cues.Where(x => x.Name == in_name).FirstOrDefault()!;
        }

        public Cue[] GetCues()
        {
            return [.. _soundBank.Data.Cues];
        }

        public void SetCue(string in_name, uint in_category, float in_unknownSingle, float in_radius, string in_stream)
        {
            if (_soundBank.Data.Cues.Any(x => x.Name == in_name))
            {
                var cue = GetCue(in_name);

                cue.Category = in_category;
                cue.UnknownSingle = in_unknownSingle;
                cue.Radius = in_radius;
                cue.Stream = in_stream;
            }
            else
            {
                _soundBank.Data.Cues.Add(new(in_name, in_category, in_unknownSingle, in_radius, in_stream));
            }
        }

        public void SetCue(DynValue in_table)
        {
            var cue = in_table.ParseClassFromDynValue<Cue>();
            var index = _soundBank.Data.Cues.FindIndex(x => x.Name == cue.Name);

            if (index == -1)
            {
                _soundBank.Data.Cues.Add(cue);
            }
            else
            {
                _soundBank.Data.Cues[index] = cue;
            }
        }

        public void Save()
        {
            Save(_soundBank);
        }
    }
}

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
                // TODO: Give Marathon Cue Constructors and crush this down like in the Message Table Wrapper.
                var cue = new Cue()
                {
                    Name = in_name,
                    Category = in_category,
                    UnknownSingle = in_unknownSingle,
                    Radius = in_radius,
                    Stream = in_stream
                };

                _soundBank.Data.Cues.Add(cue);
            }
        }

        public void Save()
        {
            Save(_soundBank);
        }
    }
}

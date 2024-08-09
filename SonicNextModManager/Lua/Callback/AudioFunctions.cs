using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Wrappers;
using SonicNextModManager.Lua.Wrappers.Audio;

namespace SonicNextModManager.Lua.Callback
{
    public class AudioFunctions
    {
        [LuaCallback]
        public static DynValue LoadSoundBank(string in_path)
        {
            return MarathonWrapper.RegisterWrapperToArchiveFile<SoundBankWrapper>(in_path);
        }
    }
}

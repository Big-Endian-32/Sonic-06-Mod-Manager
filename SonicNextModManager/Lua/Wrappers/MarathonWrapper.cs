using Marathon.Formats.Archive;
using Marathon.IO;
using SonicNextModManager.Helpers;

namespace SonicNextModManager.Lua.Wrappers
{
    public class MarathonWrapper
    {
        protected U8ArchiveFile File { get; private set; }

        public MarathonWrapper() { }

        public MarathonWrapper(U8ArchiveFile in_file)
        {
            File = in_file;
        }

        public void Save<T>(T in_instance) where T : FileBase
        {
            File.Data = IOHelper.GetMarathonTypeBuffer(in_instance);
        }
    }
}

using Marathon.Formats.Archive;
using Marathon.Helpers;
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

        public static DynValue RegisterWrapper<T>(U8Archive in_arc, string in_path) where T : MarathonWrapper
        {
            var file = in_arc.Root.GetFile(in_path);

            if (file == null)
                return DynValue.Nil;

            file.Decompress();

            var instance = Activator.CreateInstance(typeof(T), file);

            if (instance == null)
                return DynValue.Nil;

            return UserData.Create((T)instance);
        }

        public void Save<T>(T in_instance) where T : FileBase
        {
            File.Data = IOHelper.GetMarathonTypeBuffer(in_instance);
        }
    }
}

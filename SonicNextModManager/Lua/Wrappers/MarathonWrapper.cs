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

        public static DynValue RegisterWrapperToArchiveFile<T>(string in_path) where T : MarathonWrapper
        {
            var file = ArchiveHelper.GetArchiveFile(in_path);

            if (file == null)
                return DynValue.Nil;

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

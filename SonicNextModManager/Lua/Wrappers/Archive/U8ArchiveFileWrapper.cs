using Marathon.Formats.Archive;
using Marathon.Helpers;
using SonicNextModManager.Lua.Attributes;

namespace SonicNextModManager.Lua.Wrappers.Archive
{
    [LuaUserData]
    public class U8ArchiveFileWrapper : FileWrapper
    {
        private readonly U8ArchiveFile _file;

        public U8ArchiveFileWrapper() { }

        public U8ArchiveFileWrapper(U8Archive in_arc, string in_path)
        {
            Path = in_path;

            _file = (U8ArchiveFile)in_arc.Root.GetFile(in_path);

            if (_file == null)
                return;

            _file.Decompress();

            Stream = new MemoryStream(_file.Data);
            Memory = new(Stream);
        }
    }
}

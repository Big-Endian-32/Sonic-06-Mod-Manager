using Marathon.Formats.Archive;
using Marathon.Helpers;
using SonicNextModManager.Lua.Attributes;

namespace SonicNextModManager.Lua.Wrappers
{
    [LuaUserData]
    public class BufferWrapper
    {
        private readonly U8ArchiveFile _file;

        private readonly MemoryStream _stream;

        public BufferWrapper() { }

        public BufferWrapper(U8Archive in_arc, string in_path, EReadMode in_readMode = EReadMode.Stream)
        {
            _file = (U8ArchiveFile)in_arc.Root.GetFile(in_path);

            if (_file == null)
                return;

            _file.Decompress();

            var buffer = in_readMode == EReadMode.Stream
                ? _file.Data
                : (byte[])_file.Data.Clone();

            _stream = new(buffer);
        }

        public string ReadBytes(uint in_addr, int in_count)
        {
            return MemoryFramework.ReadBytes(_stream, in_addr, in_count);
        }

        public bool WriteBytes(uint in_addr, byte[] in_data)
        {
            return MemoryFramework.WriteBytes(_stream, in_addr, in_data);
        }

        public bool WriteBytes(uint in_addr, string in_hexStr)
        {
            return MemoryFramework.WriteBytes(_stream, in_addr, in_hexStr);
        }

        public bool WriteNulls(uint in_addr, int in_count)
        {
            return MemoryFramework.WriteNulls(_stream, in_addr, in_count);
        }

        public string WriteString(uint in_addr, string in_str, string in_encoding = "utf-8")
        {
            return MemoryFramework.WriteString(_stream, in_addr, in_str, in_encoding);
        }

        public string WriteUnicodeString(uint in_addr, string in_str, bool in_isBigEndian = true)
        {
            return MemoryFramework.WriteUnicodeString(_stream, in_addr, in_str, in_isBigEndian);
        }

        public void Save()
        {
            _file.Data = _stream.ToArray();
        }
    }
}

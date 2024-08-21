using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;

namespace SonicNextModManager.Lua.Wrappers
{
    [LuaUserData]
    public class FileWrapper
    {
        public Stream Stream { get; protected set; }

        public MemoryService Memory { get; protected set; }

        public string Path { get; protected set; }

        public FileWrapper() { }

        public FileWrapper(string in_path)
        {
            Path = in_path;

            Stream = new FileStream(in_path, FileMode.Open, FileAccess.ReadWrite);
            Memory = new(Stream);
        }

        public byte this[uint in_addr]
        {
            get => Memory[in_addr];
        }

        public object Read(string in_type, uint in_addr, bool in_isBigEndian = true)
        {
            if (!TypeHelper.TryParse(in_type, out var out_type) || out_type == null)
                return false;

            var value = Memory.Read(in_addr, out_type, in_isBigEndian);

            if (value == null)
                return false;

            return Convert.ChangeType(value, out_type);
        }

        public string ReadBytes(uint in_addr, int in_count)
        {
            return Memory.ReadBytes(in_addr, in_count);
        }

        public bool Write(string in_type, uint in_addr, object in_value, bool in_isBigEndian = true)
        {
            if (!TypeHelper.TryParse(in_type, out var out_type) || out_type == null)
                return false;

            in_value = Convert.ChangeType(in_value, out_type);

            return Memory.Write(in_addr, in_value, in_isBigEndian);
        }

        public bool WriteBytes(uint in_addr, byte[] in_data)
        {
            return Memory.WriteBytes(in_addr, in_data);
        }

        public bool WriteBytes(uint in_addr, string in_hexStr)
        {
            return Memory.WriteBytes(in_addr, in_hexStr);
        }

        public bool WriteNullBytes(uint in_addr, int in_count)
        {
            return Memory.WriteNullBytes(in_addr, in_count);
        }

        public string WriteString(uint in_addr, string in_str, string in_encoding = "utf-8")
        {
            return Memory.WriteString(in_addr, in_str, in_encoding);
        }

        public string WriteUnicodeString(uint in_addr, string in_str, bool in_isBigEndian = true)
        {
            return Memory.WriteUnicodeString(in_addr, in_str, in_isBigEndian);
        }

        public void Close()
        {
            Stream.Dispose();
        }
    }
}

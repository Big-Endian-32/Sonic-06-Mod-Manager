using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;

namespace SonicNextModManager.Lua.Callback
{
    public class MemoryFunctions
    {
        [LuaCallback]
        public static string ReadBytes(string in_file, uint in_addr, int in_count)
        {
            using (var fs = File.OpenRead(Patcher.GetSymbol(in_file)))
                return MemoryFramework.ReadBytes(fs, in_addr, in_count);
        }

        [LuaCallback]
        public static bool WriteBytes(string in_file, uint in_addr, byte[] in_data)
        {
            IOHelper.Backup(in_file);

            using (var fs = File.OpenWrite(Patcher.GetSymbol(in_file)))
                return MemoryFramework.WriteBytes(fs, in_addr, in_data);
        }

        [LuaCallback]
        public static bool WriteBytes(string in_file, uint in_addr, string in_hexStr)
        {
            IOHelper.Backup(in_file);

            using (var fs = File.OpenWrite(Patcher.GetSymbol(in_file)))
                return MemoryFramework.WriteBytes(fs, in_addr, in_hexStr);
        }

        [LuaCallback]
        public static bool WriteNulls(string in_file, uint in_addr, int in_count)
        {
            using (var fs = File.OpenWrite(Patcher.GetSymbol(in_file)))
                return MemoryFramework.WriteNulls(fs, in_addr, in_count);
        }

        [LuaCallback]
        public static string WriteString(string in_file, uint in_addr, string in_str, string in_encoding = "utf-8")
        {
            using (var fs = File.OpenWrite(Patcher.GetSymbol(in_file)))
                return MemoryFramework.WriteString(fs, in_addr, in_str, in_encoding);
        }

        [LuaCallback]
        public static string WriteUnicodeString(string in_file, uint in_addr, string in_str, bool in_isBigEndian = true)
        {
            using (var fs = File.OpenWrite(Patcher.GetSymbol(in_file)))
                return MemoryFramework.WriteUnicodeString(fs, in_addr, in_str, in_isBigEndian);
        }
    }
}

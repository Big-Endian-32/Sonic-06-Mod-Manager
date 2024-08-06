using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;

namespace SonicNextModManager.Lua.Callback
{
    public class MemoryFunctions
    {
        /// <summary>
        /// Reads a buffer from a file.
        /// </summary>
        /// <param name="in_file">The path to the file to read from.</param>
        /// <param name="in_addr">The address in the file to read from.</param>
        /// <param name="in_count">The amount of bytes to read.</param>
        /// <returns>A hexadecimal string representing the buffer.</returns>
        [LuaCallback]
        public static string ReadBytes(string in_file, uint in_addr, int in_count)
        {
#if !DEBUG
            try
            {
#endif
                // Open the file for reading.
                // TODO: accept paths to archive files.
                using (FileStream fileStream = File.OpenRead(Patcher.GetSymbol(in_file)))
                {
                    using (var reader = new BinaryReader(fileStream))
                    {
                        // Seek to requested address in the stream.
                        reader.BaseStream.Seek(in_addr, SeekOrigin.Begin);

                        // Return as a hexadecimal string.
                        return MemoryHelper.ByteArrayToHexString(reader.ReadBytes(in_count));
                    }
                }
#if !DEBUG
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return string.Empty;
            }
#endif
        }

        /// <summary>
        /// Writes a buffer to a file.
        /// </summary>
        /// <param name="in_file">The path to the file to write to.</param>
        /// <param name="in_addr">The address in the file to write to.</param>
        /// <param name="in_hexStr">The bytes to write in a hexadecimal string.</param>
        [LuaCallback]
        public static bool WriteBytes(string in_file, uint in_addr, string in_hexStr)
        {
#if !DEBUG
            try
            {
#endif
                IOHelper.Backup(in_file);

                // Open the file for writing.
                // TODO: accept paths to archive files.
                using (FileStream fileStream = File.OpenWrite(Patcher.GetSymbol(in_file)))
                {
                    using (var writer = new BinaryWriter(fileStream))
                    {
                        writer.BaseStream.Seek(in_addr, SeekOrigin.Begin);
                        writer.Write(MemoryHelper.HexStringToByteArray(in_hexStr));

#if DEBUG
                        Console.WriteLine($"Written bytes to 0x{in_addr:X8}: {in_hexStr}");
#endif
                    }
                }
#if !DEBUG
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return false;
            }
#endif

            return true;
        }

        /// <summary>
        /// Writes a buffer to a file.
        /// </summary>
        /// <param name="in_file">The path to the file to write to.</param>
        /// <param name="in_addr">The address in the file to write to.</param>
        /// <param name="in_data">The bytes to write.</param>
        [LuaCallback]
        public static bool WriteBytes(string in_file, uint in_addr, byte[] in_data)
        {
            return WriteBytes(in_file, in_addr, MemoryHelper.ByteArrayToHexString(in_data));
        }

        /// <summary>
        /// Writes null bytes to a file.
        /// </summary>
        /// <param name="in_file">The path to the file to write to.</param>
        /// <param name="in_addr">The address in the file to write to.</param>
        /// <param name="in_data">The amount of null bytes to write.</param>
        [LuaCallback]
        public static bool WriteNulls(string in_file, uint in_addr, int in_count)
        {
            for (uint i = 0; i < in_count; i++)
            {
                if (!WriteBytes(in_file, in_addr + i, [0x00]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Writes a UTF-8 string to a file.
        /// </summary>
        /// <param name="in_file">The path to the file to write to.</param>
        /// <param name="in_addr">The address in the file to write to.</param>
        /// <param name="in_str">The string to write.</param>
        /// <returns>The string that was written to the file.</returns>
        [LuaCallback]
        public static string WriteString(string in_file, uint in_addr, string in_str)
        {
            WriteBytes(in_file, in_addr, Encoding.UTF8.GetBytes(in_str));
            return in_str;
        }

        /// <summary>
        /// Writes a Unicode string to a file.
        /// </summary>
        /// <param name="in_file">The path to the file to write to.</param>
        /// <param name="in_addr">The address in the file to write to.</param>
        /// <param name="in_str">The string to write.</param>
        /// <returns>The string that was written to the file.</returns>
        [LuaCallback]
        public static string WriteUnicodeString(string in_file, uint in_addr, string in_str)
        {
            WriteBytes(in_file, in_addr, Encoding.BigEndianUnicode.GetBytes(in_str));
            return in_str;
        }
    }
}

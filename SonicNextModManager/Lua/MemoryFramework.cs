using SonicNextModManager.Helpers;

namespace SonicNextModManager.Lua
{
    public class MemoryFramework
    {
        /// <summary>
        /// Reads a buffer from a file.
        /// </summary>
        /// <param name="in_stream">The stream to read from.</param>
        /// <param name="in_addr">The address to write to.</param>
        /// <param name="in_count">The amount of bytes to read.</param>
        /// <returns>A hexadecimal string representing the buffer.</returns>
        public static string ReadBytes(Stream in_stream, uint in_addr, int in_count)
        {
            if (in_stream.Length < in_addr + in_count)
                return string.Empty;

            using (var reader = new BinaryReader(in_stream))
            {
                reader.BaseStream.Seek(in_addr, SeekOrigin.Begin);

                return MemoryHelper.ByteArrayToHexString(reader.ReadBytes(in_count));
            }
        }

        /// <summary>
        /// Writes a buffer to a file.
        /// </summary>
        /// <param name="in_stream">The stream to write to.</param>
        /// <param name="in_addr">The address to write to.</param>
        /// <param name="in_data">The bytes to write.</param>
        public static bool WriteBytes(Stream in_stream, uint in_addr, byte[] in_data)
        {
            if (in_stream.Length < in_addr + in_data.Length)
                return false;

            using (var writer = new BinaryWriter(in_stream))
            {
                writer.BaseStream.Seek(in_addr, SeekOrigin.Begin);
                writer.Write(in_data);
            }

            return true;
        }

        /// <summary>
        /// Writes a buffer to a file from a hexadecimal string.
        /// </summary>
        /// <param name="in_stream">The stream to write to.</param>
        /// <param name="in_addr">The address to write to.</param>
        /// <param name="in_hexStr">The bytes to write in a hexadecimal string.</param>
        public static bool WriteBytes(Stream in_stream, uint in_addr, string in_hexStr)
        {
            return WriteBytes(in_stream, in_addr, MemoryHelper.HexStringToByteArray(in_hexStr));
        }

        /// <summary>
        /// Writes null bytes to a file.
        /// </summary>
        /// <param name="in_stream">The stream to write to.</param>
        /// <param name="in_addr">The address to write to.</param>
        /// <param name="in_count">The amount of null bytes to write.</param>
        public static bool WriteNulls(Stream in_stream, uint in_addr, int in_count)
        {
            var buffer = new byte[in_count];

            if (!WriteBytes(in_stream, in_addr, buffer))
                return false;

            return true;
        }

        /// <summary>
        /// Writes a UTF-8 string to a file.
        /// </summary>
        /// <param name="in_stream">The stream to write to.</param>
        /// <param name="in_addr">The address to write to.</param>
        /// <param name="in_str">The string to write.</param>
        /// <returns>The string that was written to the file.</returns>
        public static string WriteString(Stream in_stream, uint in_addr, string in_str, string in_encoding = "utf-8")
        {
            WriteBytes(in_stream, in_addr, Encoding.GetEncoding(in_encoding).GetBytes(in_str));
            return in_str;
        }

        /// <summary>
        /// Writes a Unicode string to a file.
        /// </summary>
        /// <param name="in_stream">The stream to write to.</param>
        /// <param name="in_addr">The address to write to.</param>
        /// <param name="in_str">The string to write.</param>
        /// <returns>The string that was written to the file.</returns>
        public static string WriteUnicodeString(Stream in_stream, uint in_addr, string in_str, bool in_isBigEndian = true)
        {
            WriteString(in_stream, in_addr, in_str, in_isBigEndian ? "utf-16BE" : "utf-16");
            return in_str;
        }
    }
}

using SonicNextModManager.Helpers;
using System.Runtime.InteropServices;

namespace SonicNextModManager.Services
{
    public class MemoryService(Stream in_stream)
    {
        public byte this[uint in_addr]
        {
            get => Read<byte>(in_addr);
        }

        /// <summary>
        /// Gets the stream buffer as a byte array.
        /// </summary>
        public byte[] ToArray()
        {
            if (in_stream is MemoryStream out_ms)
                return out_ms.ToArray();

            throw new NotSupportedException();
        }

        /// <summary>
        /// Reads an unmanaged type from the specified location.
        /// </summary>
        /// <param name="in_addr">The address to read from.</param>
        /// <param name="in_type">The type of the value to read.</param>
        /// <param name="in_isBigEndian">Determines whether to read the value in big-endian byte order.</param>
        public object Read(uint in_addr, Type in_type, bool in_isBigEndian = true)
        {
            var buffer = ReadBytesBuffer(in_addr, Marshal.SizeOf(in_type));

            return MemoryHelper.ByteArrayToUnmanagedType(buffer, in_type, in_isBigEndian)!;
        }

        /// <summary>
        /// Reads an unmanaged type from the specified location.
        /// </summary>
        /// <typeparam name="T">The type of the value to read.</typeparam>
        /// <param name="in_addr">The address to read from.</param>
        /// <param name="in_isBigEndian">Determines whether to read the value in big-endian byte order.</param>
        public T Read<T>(uint in_addr, bool in_isBigEndian = true)
        {
            return (T)Read(in_addr, typeof(T), in_isBigEndian);
        }

        /// <summary>
        /// Reads a buffer from the specified location.
        /// </summary>
        /// <param name="in_addr">The address to read from.</param>
        /// <param name="in_count">The amount of bytes to read.</param>
        /// <returns>A byte array representing the buffer.</returns>
        public byte[] ReadBytesBuffer(uint in_addr, int in_count)
        {
            if (in_stream.Length < in_addr + in_count)
                return [];

            var buffer = new byte[in_count];

            in_stream.Seek(in_addr, SeekOrigin.Begin);

            using (var reader = new BinaryReader(in_stream))
                reader.Read(buffer, 0, buffer.Length);

            return buffer;
        }

        /// <summary>
        /// Reads a buffer from the specified location.
        /// </summary>
        /// <param name="in_addr">The address to read from.</param>
        /// <param name="in_count">The amount of bytes to read.</param>
        /// <returns>A hexadecimal string representing the buffer.</returns>
        public string ReadBytes(uint in_addr, int in_count)
        {
            return MemoryHelper.ByteArrayToHexString(ReadBytesBuffer(in_addr, in_count));
        }

        /// <summary>
        /// Writes an unmanaged type to the specified location.
        /// </summary>
        /// <typeparam name="T">The type of the value to write.</typeparam>
        /// <param name="in_addr">The address to write to.</param>
        /// <param name="in_value">The value to write.</param>
        /// <param name="in_isBigEndian">Determines whether to write the value in big-endian byte order.</param>
        public bool Write(uint in_addr, object in_value, bool in_isBigEndian = true)
        {
            return WriteBytes(in_addr, MemoryHelper.UnmanagedTypeToByteArray(in_value, in_isBigEndian));
        }

        /// <summary>
        /// Writes a buffer to the specified location.
        /// </summary>
        /// <param name="in_addr">The address to write to.</param>
        /// <param name="in_data">The bytes to write.</param>
        public bool WriteBytes(uint in_addr, byte[] in_data)
        {
            if (in_stream.Length < in_addr + in_data.Length)
                return false;

            in_stream.Seek(in_addr, SeekOrigin.Begin);
            in_stream.Write(in_data);

            return true;
        }

        /// <summary>
        /// Writes a buffer to the specified location from a hexadecimal string.
        /// </summary>
        /// <param name="in_addr">The address to write to.</param>
        /// <param name="in_hexStr">The bytes to write in a hexadecimal string.</param>
        public bool WriteBytes(uint in_addr, string in_hexStr)
        {
            return WriteBytes(in_addr, MemoryHelper.HexStringToByteArray(in_hexStr));
        }

        /// <summary>
        /// Writes null bytes to the specified location.
        /// </summary>
        /// <param name="in_addr">The address to write to.</param>
        /// <param name="in_count">The amount of null bytes to write.</param>
        public bool WriteNullBytes(uint in_addr, int in_count)
        {
            var buffer = new byte[in_count];

            if (!WriteBytes(in_addr, buffer))
                return false;

            return true;
        }

        /// <summary>
        /// Writes a UTF-8 string to the specified location.
        /// </summary>
        /// <param name="in_addr">The address to write to.</param>
        /// <param name="in_str">The string to write.</param>
        /// <returns>The string that was written to the file.</returns>
        public string WriteString(uint in_addr, string in_str, string in_encoding = "utf-8")
        {
            WriteBytes(in_addr, Encoding.GetEncoding(in_encoding).GetBytes(in_str));
            return in_str;
        }

        /// <summary>
        /// Writes a Unicode string to the specified location.
        /// </summary>
        /// <param name="in_addr">The address to write to.</param>
        /// <param name="in_str">The string to write.</param>
        /// <returns>The string that was written to the file.</returns>
        public string WriteUnicodeString(uint in_addr, string in_str, bool in_isBigEndian = true)
        {
            WriteString(in_addr, in_str, in_isBigEndian ? "utf-16BE" : "utf-16");
            return in_str;
        }
    }
}

using SonicNextModManager.Metadata;
using System.Runtime.InteropServices;

namespace SonicNextModManager.Helpers
{
    public class MemoryHelper
    {
        /// <summary>
        /// Returns a hexadecimal string as a byte array.
        /// </summary>
        /// <param name="in_hexStr">Hexadecimal string to convert.</param>
        public static byte[] HexStringToByteArray(string in_hexStr)
        {
            // Remove any whitespace characters for proper conversion.
            string hexNoWhitespace = in_hexStr.Replace(" ", "");

            return Enumerable.Range(0, hexNoWhitespace.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hexNoWhitespace.Substring(x, 2), 16))
                             .ToArray();
        }

        /// <summary>
        /// Returns a hexadecimal string based off a byte array.
        /// </summary>
        /// <param name="in_data">Bytes to convert.</param>
        public static string ByteArrayToHexString(byte[] in_data, bool in_isSpaced = true)
        {
            return BitConverter.ToString(in_data).Replace("-", in_isSpaced ? " " : "");
        }

        /// <summary>
        /// Transforms a virtual memory address to a physical executable address.
        /// </summary>
        /// <param name="in_addr">The address to transform.</param>
        public static uint ToPhysical(uint in_addr)
        {
            return App.GetCurrentPlatform() switch
            {
                EPlatform.Xbox => (in_addr - 0x82000000) + 0x3000,
                EPlatform.PlayStation => in_addr - 0x10000,
                _ => in_addr,
            };
        }

        /// <summary>
        /// Transforms a physical executable address to a virtual memory address.
        /// </summary>
        /// <param name="in_addr">The address to transform.</param>
        public static uint ToVirtual(uint in_addr)
        {
            return App.GetCurrentPlatform() switch
            {
                EPlatform.Xbox => (in_addr + 0x82000000) - 0x3000,
                EPlatform.PlayStation => in_addr + 0x10000,
                _ => in_addr,
            };
        }

        public static object? ByteArrayToUnmanagedType(byte[] in_data, Type in_type, bool in_isBigEndian = false)
        {
            if (in_data == null || in_data.Length <= 0)
                return null;

            if (in_isBigEndian)
                in_data = in_data.Reverse().ToArray();

            var handle = GCHandle.Alloc(in_data, GCHandleType.Pinned);

            try
            {
                return Marshal.PtrToStructure(handle.AddrOfPinnedObject(), in_type)!;
            }
            finally
            {
                handle.Free();
            }
        }

        public static T? ByteArrayToUnmanagedType<T>(byte[] in_data, bool in_isBigEndian = false)
        {
            return (T?)ByteArrayToUnmanagedType(in_data, typeof(T), in_isBigEndian);
        }

        public static byte[] UnmanagedTypeToByteArray(object in_structure, bool in_isBigEndian = false)
        {
            byte[] data = new byte[Marshal.SizeOf(in_structure.GetType())];

            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);

            try
            {
                Marshal.StructureToPtr(in_structure, handle.AddrOfPinnedObject(), false);
            }
            finally
            {
                handle.Free();
            }

            return in_isBigEndian ? data.Reverse().ToArray() : data;
        }
    }
}

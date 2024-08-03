namespace SonicNextModManager.Helpers
{
    public class MemoryHelper
    {
        /// <summary>
        /// Returns a hexadecimal string as a byte array.
        /// </summary>
        /// <param name="hex">Hexadecimal string to convert.</param>
        public static byte[] HexStringToByteArray(string hex)
        {
            // Remove any whitespace characters for proper conversion.
            string hexNoWhitespace = hex.Replace(" ", "");

            return Enumerable.Range(0, hexNoWhitespace.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hexNoWhitespace.Substring(x, 2), 16))
                             .ToArray();
        }

        /// <summary>
        /// Returns a hexadecimal string based off a byte array.
        /// </summary>
        /// <param name="data">Bytes to convert.</param>
        public static string ByteArrayToHexString(byte[] data)
            => BitConverter.ToString(data).Replace("-", " ");
    }
}

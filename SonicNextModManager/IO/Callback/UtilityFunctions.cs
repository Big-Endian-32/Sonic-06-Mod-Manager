using SonicNextModManager.Helpers;

namespace SonicNextModManager.IO.Callback
{
    [MoonSharpUserData]
    public class UtilityFunctions
    {
        /// <summary>
        /// Returns a UTF8 string converted from hexadecimal.
        /// </summary>
        /// <param name="hex">Hexadecimal to convert.</param>
        public static string ToString(string hex)
            => Encoding.UTF8.GetString(MemoryHelper.HexStringToByteArray(hex));

        /// <summary>
        /// Returns the value of a symbol.
        /// <para>If the symbol does not exist, it'll return the input string.</para>
        /// </summary>
        /// <param name="symbol">Symbol name.</param>
        public static string GetSymbol(string symbol)
            => Patcher.GetSymbol(symbol);
    }
}

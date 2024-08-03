using SonicNextModManager.Helpers;
using SonicNextModManager.Metadata;

namespace SonicNextModManager.Lua.Callback
{
    [MoonSharpUserData]
    public class UtilityFunctions
    {
        /// <summary>
        /// Returns a UTF8 string converted from hexadecimal.
        /// </summary>
        /// <param name="in_hexStr">Hexadecimal to convert.</param>
        public static string ToString(string in_hexStr)
            => Encoding.UTF8.GetString(MemoryHelper.HexStringToByteArray(in_hexStr));

        /// <summary>
        /// Transforms a virtual memory address to a physical executable address.
        /// </summary>
        /// <param name="in_addr">The address to transform.</param>
        public static uint ToPhysical(uint in_addr)
        {
            switch (App.CurrentPlatform)
            {
                case Platform.Xbox:
                    return (in_addr - 0x82000000) + 0x3000;

                case Platform.PlayStation:
                    return in_addr - 0x10000;
            }

            return in_addr;
        }

        /// <summary>
        /// Transforms a physical executable address to a virtual memory address.
        /// </summary>
        /// <param name="in_addr">The address to transform.</param>
        public static uint ToVirtual(uint in_addr)
        {
            switch (App.CurrentPlatform)
            {
                case Platform.Xbox:
                    return (in_addr + 0x82000000) - 0x3000;

                case Platform.PlayStation:
                    return in_addr + 0x10000;
            }

            return in_addr;
        }

        /// <summary>
        /// Returns the value of a symbol.
        /// <para>If the symbol does not exist, it'll return the input string.</para>
        /// </summary>
        /// <param name="in_symbol">Symbol name.</param>
        public static string GetSymbol(string in_symbol)
            => Patcher.GetSymbol(in_symbol);
    }
}

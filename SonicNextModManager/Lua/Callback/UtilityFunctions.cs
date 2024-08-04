using SonicNextModManager.Helpers;
using SonicNextModManager.Metadata;

namespace SonicNextModManager.Lua.Callback
{
    [MoonSharpUserData]
    public class UtilityFunctions
    {
        /// <summary>
        /// Transforms a hexadecimal string to UTF-8 string.
        /// </summary>
        /// <param name="in_hexStr">The hexadecimal string with UTF-8 bytes.</param>
        public static string ToString(string in_hexStr)
        {
            return Encoding.UTF8.GetString(MemoryHelper.HexStringToByteArray(in_hexStr));
        }

        /// <summary>
        /// Transforms a virtual memory address to a physical executable address.
        /// </summary>
        /// <param name="in_addr">The address to transform.</param>
        public static uint ToPhysical(uint in_addr)
        {
            switch (App.GetCurrentPlatform())
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
            switch (App.GetCurrentPlatform())
            {
                case Platform.Xbox:
                    return (in_addr + 0x82000000) - 0x3000;

                case Platform.PlayStation:
                    return in_addr + 0x10000;
            }

            return in_addr;
        }

        /// <summary>
        /// Gets the value of a symbol.
        /// </summary>
        /// <param name="in_symbol">The name of the symbol to get.</param>
        /// <returns>If it exists, the value of the specified symbol; otherwise, the symbol name.</returns>
        public static string GetSymbol(string in_symbol)
        {
            return Patcher.GetSymbol(in_symbol);
        }

        /// <summary>
        /// Combines an array of strings into a path.
        /// </summary>
        /// <param name="in_paths">An array of parts of the path.</param>
        /// <returns>The combined paths.</returns>
        public static string PathCombine(params string[] in_paths)
        {
            for (int i = 0; i < in_paths.Length; i++)
            {
                if (in_paths[i].Contains('\\'))
                    in_paths[i].Replace('\\', '/');
            }

            return Path.Combine(in_paths);
        }
    }
}

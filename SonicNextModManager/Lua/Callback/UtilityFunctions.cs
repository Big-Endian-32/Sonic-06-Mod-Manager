using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Metadata;
using SonicNextModManager.UI.Dialogs;

namespace SonicNextModManager.Lua.Callback
{
    public class UtilityFunctions
    {
        /// <summary>
        /// Transforms a hexadecimal string to UTF-8 string.
        /// </summary>
        /// <param name="in_hexStr">The hexadecimal string with UTF-8 bytes.</param>
        [LuaCallback]
        public static string ToString(string in_hexStr)
        {
            return Encoding.UTF8.GetString(MemoryHelper.HexStringToByteArray(in_hexStr));
        }

        /// <summary>
        /// Transforms a virtual memory address to a physical executable address.
        /// </summary>
        /// <param name="in_addr">The address to transform.</param>
        [LuaCallback]
        public static uint ToPhysical(uint in_addr)
        {
            return App.GetCurrentPlatform() switch
            {
                EPlatform.Xbox        => (in_addr - 0x82000000) + 0x3000,
                EPlatform.PlayStation => in_addr - 0x10000,
                _                    => in_addr,
            };
        }

        /// <summary>
        /// Transforms a physical executable address to a virtual memory address.
        /// </summary>
        /// <param name="in_addr">The address to transform.</param>
        [LuaCallback]
        public static uint ToVirtual(uint in_addr)
        {
            return App.GetCurrentPlatform() switch
            {
                EPlatform.Xbox        => (in_addr + 0x82000000) - 0x3000,
                EPlatform.PlayStation => in_addr + 0x10000,
                _                    => in_addr,
            };
        }

        /// <summary>
        /// Gets the value of a symbol.
        /// </summary>
        /// <param name="in_symbol">The name of the symbol to get.</param>
        /// <returns>If it exists, the value of the specified symbol; otherwise, the symbol name.</returns>
        [LuaCallback]
        public static string GetSymbol(string in_symbol)
        {
            return Patcher.GetSymbol(in_symbol);
        }

        /// <summary>
        /// Combines an array of strings into a path.
        /// </summary>
        /// <param name="in_paths">An array of parts of the path.</param>
        /// <returns>The combined paths.</returns>
        [LuaCallback]
        public static string PathCombine(params string[] in_paths)
        {
            for (int i = 0; i < in_paths.Length; i++)
            {
                if (in_paths[i].Contains('\\'))
                    in_paths[i] = in_paths[i].Replace('\\', '/');
            }

            return Path.Combine(in_paths);
        }

        /// <summary>
        /// Displays a message box to the user.
        /// </summary>
        /// <param name="in_message">The message to display.</param>
        /// <param name="in_caption">The caption to display.</param>
        /// <param name="in_buttons">The buttons to display.</param>
        /// <param name="in_icon">The icon to display.</param>
        /// <returns>The result of the message box (e.g. which button the user clicked).</returns>
        [LuaCallback]
        public static string MessageBox
        (
            string in_message,
            string in_caption = "Sonic '06 Mod Manager",
            string in_buttons = "OK",
            string in_icon = "Information"
        )
        {
            if (!Enum.TryParse(typeof(ENextMessageBoxButton), in_buttons, true, out var out_buttons))
                out_buttons = ENextMessageBoxButton.OK;

            if (!Enum.TryParse(typeof(ENextMessageBoxIcon), in_icon, true, out var out_icon))
                out_icon = ENextMessageBoxIcon.Information;

            return NextMessageBox.Show(in_message, in_caption, (ENextMessageBoxButton)out_buttons, (ENextMessageBoxIcon)out_icon).ToString();
        }
    }
}

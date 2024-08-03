using System.Runtime.InteropServices;

namespace SonicNextModManager
{
    public static class ImmersiveDarkMode
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        /// <summary>
        /// Initialises immersive dark mode for the input handle in Desktop Window Manager.
        /// <para>This requires an application manifest that supports Windows 10 for OSVersion to return the correct build numbers.</para>
        /// </summary>
        /// <param name="handle">Window handle to apply immersive dark mode to.</param>
        /// <param name="enabled">Whether or not immersive dark mode should be enabled.</param>
        public static bool Initialise(IntPtr handle, bool enabled)
        {
            int useImmersiveDarkMode = enabled ? 1 : 0;

            if (IsW10OrGreater(17763))
            {
                var attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;

                if (IsW10OrGreater(18985))
                    attribute = DWMWA_USE_IMMERSIVE_DARK_MODE;

                return DwmSetWindowAttribute(handle, attribute, ref useImmersiveDarkMode, sizeof(int)) == 0;
            }

            return false;
        }

        /// <summary>
        /// Checks if the returned OSVersion is greater than Windows 10.
        /// </summary>
        /// <param name="build">Windows 10 build number to check.</param>
        private static bool IsW10OrGreater(int build)
            => Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= build;
    }
}

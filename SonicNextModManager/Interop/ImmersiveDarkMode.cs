using System.Runtime.InteropServices;

namespace SonicNextModManager.Interop
{
    public static class ImmersiveDarkMode
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr in_handle, int in_attr, ref int in_attrValue, int in_attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        /// <summary>
        /// Initialises immersive dark mode for the input handle in Desktop Window Manager.
        /// <para>This requires an application manifest that supports Windows 10 for OSVersion to return the correct build numbers.</para>
        /// </summary>
        /// <param name="in_handle">The handle of the window to apply immersive dark mode to.</param>
        /// <param name="in_isEnabled">Determines whether immersive dark mode should be enabled.</param>
        public static bool Init(IntPtr in_handle, bool in_isEnabled)
        {
            int isImmersiveDarkMode = in_isEnabled ? 1 : 0;

            if (IsW10OrGreater(17763))
            {
                var attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;

                if (IsW10OrGreater(18985))
                    attribute = DWMWA_USE_IMMERSIVE_DARK_MODE;

                return DwmSetWindowAttribute(in_handle, attribute, ref isImmersiveDarkMode, sizeof(int)) == 0;
            }

            return false;
        }

        /// <summary>
        /// Gets whether the specified OSVersion is greater than Windows 10.
        /// </summary>
        /// <param name="in_build">The Windows 10 build number to check.</param>
        private static bool IsW10OrGreater(int in_build)
        {
            return Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= in_build;
        }
    }
}
